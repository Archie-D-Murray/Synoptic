using System;
using System.Collections.Generic;

using AI.Adapters;
using AI.Injectors;

using UnityEngine;

namespace AI.HSM {

    [Serializable]
    public class AIStateView {
        public AIState Key;
        public AIState Parent;
        [HideInInspector]
        public State State = null;
    }

    class StateNode {
        public StateNode Parent;
        public State Value;
        public AIState Type;
        public List<StateNode> Children;
    }

    [Serializable]
    public class StateMachineContext {
        public MovementAdapter Movement;
        public AnimationAdapter Animator;
        public StateMachine StateMachine;
        public Transform Self;
        public DetectorAdapter Detector;

        public IIdleInjector IdleInjector { get; protected set; }
        public IWanderInjector WanderInjector { get; protected set; }
        public IPatrolInjector PatrolInjector { get; protected set; }
        public IChaseInjector ChaseInjector { get; protected set; }
        public IAttackInjector AttackInjector { get; protected set; }
        public Vector3 Position => Self.position;

        [SerializeField] private AIStateView[] _array;
        private Dictionary<AIState, int> _lookup = new Dictionary<AIState, int>();

        private State GetState(AIState state) {
            if (_lookup.TryGetValue(state, out int index)) {
                return _array[index].State;
            } else {
                return null;
            }
        }

        public State this[AIState state] {
            get {
                return GetState(state);
            }
            private set {
                if (_lookup.TryGetValue(state, out int index)) {
                    _array[index].State = value;
                }
            }
        }

        public void Init(Action injectorInitOverride = null) {
            int i = 0;
            foreach (AIStateView view in _array) {
                _lookup.Add(view.Key, i);
                i++;
            }
            StateFactory factory = new StateFactory(StateMachine, this);
            HashSet<State> created = new HashSet<State>();

            foreach (AIStateView view in _array) {
                InitState(view, factory);
            }

            if (injectorInitOverride != null) {
                injectorInitOverride.Invoke();
            } else {
                InitDefaultInjectors();
            }
        }

        private void InitState(AIStateView view, StateFactory factory) {
            if (view.Key == AIState.None) {
                return;
            }
            if (view.Key == AIState.Root) {
                view.State = factory.Create(AIState.Root);
                return;
            }
            if (this[view.Parent] == null) {
                InitState(_array[_lookup[view.Parent]], factory);
            }
            this[view.Key] = factory.Create(view.Key, this[view.Parent]);
        }

        private bool Patrol() {
            return IdleInjector.DoneIdling() && UnityEngine.Random.value >= 0.5f;
        }

        private bool Wander() {
            return IdleInjector.DoneIdling() && UnityEngine.Random.value < 0.5f;
        }

        private void GetDefaultTransitions() {
            // Idle
            StateMachine.AddStateTransition(GetState(AIState.Idle), GetState(AIState.Wander), new LambdaPredicate(Wander));
            StateMachine.AddStateTransition(GetState(AIState.Idle), GetState(AIState.Patrol), new LambdaPredicate(Patrol));
            StateMachine.AddStateTransition(GetState(AIState.Idle), GetState(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Wander
            StateMachine.AddStateTransition(GetState(AIState.Wander), GetState(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Patrol
            StateMachine.AddStateTransition(GetState(AIState.Patrol), GetState(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Chase
            StateMachine.AddStateTransition(GetState(AIState.Chase), GetState(AIState.Attack), new LambdaPredicate(ChaseInjector.SwitchToAttack));

            // Attack
            StateMachine.AddStateTransition(GetState(AIState.Attack), GetState(AIState.Chase), new LambdaPredicate(AttackInjector.SwitchToChase));
        }

        public void InitDefaultInjectors() {
            IdleInjector = Self.GetComponentInChildren<RandomIdleInjector>();
            WanderInjector = Self.GetComponentInChildren<WanderInjector>();
            PatrolInjector = Self.GetComponentInChildren<CyclePatrolInjector>();
            ChaseInjector = Self.GetComponentInChildren<ChaseInjector>();
            AttackInjector = Self.GetComponentInChildren<AttackInjector>();
            IdleInjector.ProvideState(this);
            WanderInjector.ProvideState(this);
            PatrolInjector.ProvideState(this);
            ChaseInjector.ProvideState(this);
            AttackInjector.ProvideState(this);
        }
    }
}
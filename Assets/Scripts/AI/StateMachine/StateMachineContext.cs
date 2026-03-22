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

        [SerializeField] private AIStateView[] _array = new AIStateView[] { new AIStateView() { Key = AIState.Root, Parent = AIState.None } };
        private Dictionary<AIState, int> _lookup = new Dictionary<AIState, int>();

        private State Get(AIState state) {
            if (_lookup.TryGetValue(state, out int index)) {
                return _array[index].State;
            } else {
                return null;
            }
        }

        public State this[AIState state] {
            get {
                return Get(state);
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

        private void GetDefaultTransitions() {
            // Idle
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Wander), new AndPredicate(new LambdaPredicate(IdleInjector.DoneIdling), new RandomChancePredicate(0.5f)));
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Patrol), new AndPredicate(new LambdaPredicate(IdleInjector.DoneIdling), new RandomChancePredicate(0.5f)));
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Wander
            StateMachine.AddStateTransition(Get(AIState.Wander), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Patrol
            StateMachine.AddStateTransition(Get(AIState.Patrol), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Chase
            StateMachine.AddStateTransition(Get(AIState.Chase), Get(AIState.Attack), new LambdaPredicate(ChaseInjector.SwitchToAttack));

            // Attack
            StateMachine.AddStateTransition(Get(AIState.Attack), Get(AIState.Chase), new LambdaPredicate(AttackInjector.SwitchToChase));
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
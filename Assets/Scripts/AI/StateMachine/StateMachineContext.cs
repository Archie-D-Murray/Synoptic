using System;
using System.Collections.Generic;
using System.Linq;

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

    public class StateMachineContext : MonoBehaviour {
        public MovementAdapter Movement;
        public AnimationAdapter Animator;
        public StateMachine StateMachine;
        public Transform Self;
        public DetectorAdapter Detector;
        public AICooldownManager CooldownManager = new AICooldownManager(new AICooldown[] {
            new AICooldown("WaitTime", 1.0f),
            new AICooldown("WanderTimer", 4.0f),
            new AICooldown("TimePerPoint", 2.0f),
            new AICooldown("LostTargetTimer", 3.0f),
            new AICooldown("Attack", 1.0f),
        });

        [SerializeField] protected string _statePath;

        public IIdleInjector IdleInjector { get; protected set; }
        public IWanderInjector WanderInjector { get; protected set; }
        public IPatrolInjector PatrolInjector { get; protected set; }
        public IChaseInjector ChaseInjector { get; protected set; }
        public IAttackInjector AttackInjector { get; protected set; }
        public Vector3 Position => Self.position;

        [SerializeField] protected AIStateView[] _array = new AIStateView[] { new AIStateView() { Key = AIState.Root, Parent = AIState.None } };
        protected Dictionary<AIState, int> _lookup = new Dictionary<AIState, int>();

        protected State Get(AIState state) {
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

        protected virtual void OnValidate() {
            Self = transform;
            Animator = GetComponentInChildren<AnimationAdapter>();
            Movement = GetComponentInChildren<MovementAdapter>();
            Detector = GetComponentInChildren<DetectorAdapter>();
        }

        protected void Awake() {
            Init();
        }

        protected virtual void Init() {
            int i = 0;

            StateMachine = new StateMachine();
            StateFactory factory = new StateFactory(StateMachine, this);
            StateMachine.SetRoot(factory.Create(AIState.Root, null));

            foreach (AIStateView view in _array) {
                _lookup.Add(view.Key, i);
                i++;
            }
            Dictionary<AIState, int> visited = new Dictionary<AIState, int>();

            foreach (AIStateView view in _array) {
                InitState(view, factory, visited);
            }

            InitInjectors();
            GetTransitions();
        }

        protected void InitState(AIStateView view, StateFactory factory, Dictionary<AIState, int> visited) {
            if (view.Key == AIState.None) {
                return;
            }
            if (visited.GetValueOrDefault(view.Key, 0) > 1) {
                Debug.LogError($"Visited state {view.State} more than once - is there a cyclical dependency?");
                return;
            }
            if (view.Key == AIState.Root) {
                view.State = StateMachine.Root;
                return;
            }
            if (this[view.Parent] == null) {
                InitState(_array[_lookup[view.Parent]], factory, visited);
            }
            if (visited.ContainsKey(view.Key)) {
                visited[view.Key]++;
            } else {
                visited.Add(view.Key, 1);
            }
            this[view.Key] = factory.Create(view.Key, this[view.Parent]);
        }

        protected void GetTransitions() {
            StateMachine.AddInitialState(Get(AIState.Root), Get(AIState.Idle));
            // Idle
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Wander), new AndPredicate(new LambdaPredicate(() => IdleInjector.DoneIdling(this)), new RandomChancePredicate(0.5f)));
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Patrol), new AndPredicate(new LambdaPredicate(() => IdleInjector.DoneIdling(this)), new RandomChancePredicate(0.5f)));
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Wander
            StateMachine.AddStateTransition(Get(AIState.Wander), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Patrol
            StateMachine.AddStateTransition(Get(AIState.Patrol), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Chase
            StateMachine.AddStateTransition(Get(AIState.Chase), Get(AIState.Attack), new LambdaPredicate(() => ChaseInjector.SwitchToAttack(this)));
            StateMachine.AddStateTransition(Get(AIState.Chase), Get(AIState.Idle), new LambdaPredicate(() => ChaseInjector.LostTarget(this)));

            // Attack
            StateMachine.AddStateTransition(Get(AIState.Attack), Get(AIState.Chase), new LambdaPredicate(() => AttackInjector.SwitchToChase(this)));
        }

        public virtual void InitInjectors() {
            IdleInjector = Self.GetComponentInChildren<RandomIdleInjector>();
            WanderInjector = Self.GetComponentInChildren<WanderInjector>();
            PatrolInjector = Self.GetComponentInChildren<CyclePatrolInjector>();
            ChaseInjector = Self.GetComponentInChildren<ChaseInjector>();
            AttackInjector = Self.GetComponentInChildren<AttackInjector>();
            IdleInjector.Init();
            WanderInjector.Init();
            PatrolInjector.Init();
            ChaseInjector.Init();
            AttackInjector.Init();
        }

        protected virtual void Update() {
            StateMachine.Tick(Time.deltaTime);
            _statePath = string.Join(" > ", StateMachine.Root.GetLeaf().PathToRoot().Reverse());
        }

        protected virtual void FixedUpdate() {
            StateMachine.FixedTick();
        }
    }
}
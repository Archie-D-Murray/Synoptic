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

    ///<summary>Base context for the state machine</summary>
    ///<summary>Add references to things like an enemy script here to use in states/injectors</summary>
    ///<summary>override things like GetTransitions to allow for different behaviours</summary>
    ///<remarks>
    ///Classes inheriting this should only override GetTransition and InitInjectors.
    ///All gameplay values should be inside this class to allow for states and injectors to acces them
    ///</remarks>
    public class StateMachineContext : MonoBehaviour {
        public MovementAdapter Movement;
        public AnimationAdapter Animator;
        public StateMachine StateMachine;
        public Transform Self;
        public DetectorAdapter Detector;
        public AICooldownManager CooldownManager = new AICooldownManager(new AICooldown[] {
            new AICooldown("WaitTime", 1.0f, false),
            new AICooldown("WanderTimer", 4.0f, false),
            new AICooldown("TimePerPoint", 2.0f, false),
            new AICooldown("LostTargetTimer", 3.0f, false),
            new AICooldown("Attack", 1.0f, false),
        });

        [SerializeField] protected string _statePath;

        public IIdleInjector IdleInjector { get; protected set; }
        public IWanderInjector WanderInjector { get; protected set; }
        public IPatrolInjector PatrolInjector { get; protected set; }
        public IChaseInjector ChaseInjector { get; protected set; }
        public IAttackInjector AttackInjector { get; protected set; }
        public Vector3 Position => Self.position;

        private bool _injectorsOnSameObj = false;

        private void CheckInjectorsOnSameObject() {
            foreach (Transform child in transform) {
                if (child.gameObject.GetComponent<IIdleInjector>() != null) {
                    _injectorsOnSameObj = child.GetComponents<IStateInjector>().Length == this.GetType().GetFields().Where(field => field.GetType().IsAssignableFrom(typeof(IStateInjector))).Count();
                    return;
                }
            }
            _injectorsOnSameObj = false;
        }

        [SerializeField] protected AIStateView[] _array = new AIStateView[] { new AIStateView() { Key = AIState.Root, Parent = AIState.None } };
        protected Dictionary<AIState, int> _lookup = new Dictionary<AIState, int>();

        ///<summary>Gets a state object from state type or null if not present</summary>
        protected State Get(AIState state) {
            if (_lookup.TryGetValue(state, out int index)) {
                return _array[index].State;
            } else {
                return null;
            }
        }

        ///<summary>Gets a state object from state type or null if not present</summary>
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
            CheckInjectorsOnSameObject();
        }

        protected void Awake() {
            Init();
        }

        ///<summary>Initialises states as dependency tree</summary>
        protected void Init() {
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
                if (view.Key == AIState.Root) { continue; }
                InitState(view, factory, visited);
            }

            InitInjectors();
            GetTransitions();
        }

        ///<summary>Initialises view.State with real state from factory resolving parents first</summary>
        ///<remarks>Root state should already be initialised</remarks>
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

        ///<summary>Provides all transitions to state machine after injectors are initialised</summary>
        ///<summary>Can be overriden by user to provide custom state data</summary>
        protected virtual void GetTransitions() {
            StateMachine.AddInitialState(Get(AIState.Root), Get(AIState.Idle));
            // Idle
            // StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Wander), new AndPredicate(new LambdaPredicate(() => IdleInjector.DoneIdling(this)), new RandomChancePredicate(0.5f)));
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Patrol), new AndPredicate(new LambdaPredicate(() => IdleInjector.DoneIdling(this)), new RandomChancePredicate(0.5f)));
            StateMachine.AddStateTransition(Get(AIState.Idle), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Wander
            StateMachine.AddStateTransition(Get(AIState.Wander), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Patrol
            StateMachine.AddStateTransition(Get(AIState.Patrol), Get(AIState.Chase), new LambdaPredicate(Detector.HasTarget));

            // Chase
            StateMachine.AddStateTransition(Get(AIState.Chase), Get(AIState.Attack), new LambdaPredicate(() => ChaseInjector.InAttackRange(this)));
            StateMachine.AddStateTransition(Get(AIState.Chase), Get(AIState.Idle), new LambdaPredicate(() => ChaseInjector.LostTarget(this)));

            // Attack
            StateMachine.AddStateTransition(Get(AIState.Attack), Get(AIState.Chase), new LambdaPredicate(() => AttackInjector.UnableToAttack(this)));
        }

        ///<summary>Provides all injectors for states to use</summary>
        ///<summary>Can be overriden to use a common set of states (maybe specific to enemy type, etc)</summary>
        public virtual void InitInjectors() {
            if (_injectorsOnSameObj) {
                GameObject injectorRoot = null;
                foreach (Transform child in transform) {
                    if (child.GetComponent<IIdleInjector>() != null) {
                        injectorRoot = child.gameObject;
                    }
                }
                IdleInjector = injectorRoot.GetComponent<IIdleInjector>();
                WanderInjector = injectorRoot.GetComponent<IWanderInjector>();
                PatrolInjector = injectorRoot.GetComponent<IPatrolInjector>();
                ChaseInjector = injectorRoot.GetComponent<IChaseInjector>();
                AttackInjector = injectorRoot.GetComponent<IAttackInjector>();
            } else {
                IdleInjector = Self.GetComponentInChildren<IIdleInjector>();
                WanderInjector = Self.GetComponentInChildren<IWanderInjector>();
                PatrolInjector = Self.GetComponentInChildren<IPatrolInjector>();
                ChaseInjector = Self.GetComponentInChildren<IChaseInjector>();
                AttackInjector = Self.GetComponentInChildren<IAttackInjector>();
            }
            IdleInjector.Init();
            WanderInjector.Init();
            PatrolInjector.Init();
            ChaseInjector.Init();
            AttackInjector.Init();
        }

        ///<summary>Updates state machine</summary>
        ///<summary>Unlikely to change unless state machine needs to not tick on Update</summary>
        protected virtual void Update() {
            float dt = Time.deltaTime;
            CooldownManager.Update(dt);
            StateMachine.Tick(dt);
            _statePath = string.Join(" > ", StateMachine.Root.GetLeaf().PathToRoot().Reverse());
        }

        ///<summary>Forwards FixedUpdate to state machine</summary>
        protected void FixedUpdate() {
            StateMachine.FixedTick();
        }
    }
}
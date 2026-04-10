using System;
using System.Collections.Generic;
using System.Linq;

using AI.Adapters;
using AI.Injectors;

using UnityEngine;

using Utilities;

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
    public sealed class StateMachineContext : MonoBehaviour {
        public MovementAdapter Movement;
        public AnimationAdapter Animator;
        public StateMachine StateMachine;
        public DetectorAdapter Detector;
        public AICooldownManager CooldownManager = new AICooldownManager(new AICooldown[] {
            new AICooldown("WaitTime", 1.0f, false),
            new AICooldown("WanderTimer", 4.0f, false),
            new AICooldown("TimePerPoint", 2.0f, false),
            new AICooldown("LostTargetTimer", 3.0f, false),
            new AICooldown("Attack", 1.0f, false),
        });

        [SerializeField] private string _statePath;

        [SerializeReference, SubclassSelector] private IStateDefinition _definition = new DefaultStateDefinitions();

        public IIdleInjector IdleInjector;
        public IWanderInjector WanderInjector;
        public IPatrolInjector PatrolInjector;
        public IChaseInjector ChaseInjector;
        public IAttackInjector AttackInjector;
        public Vector3 Position => transform.position;

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

        [SerializeField] private AIStateView[] _states = new AIStateView[] { new AIStateView() { Key = AIState.Root, Parent = AIState.None } };
        private Dictionary<AIState, int> _lookup = new Dictionary<AIState, int>();

        ///<summary>Gets a state object from state type or null if not present</summary>
        private State Get(AIState state) {
            if (_lookup.TryGetValue(state, out int index)) {
                return _states[index].State;
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
                    _states[index].State = value;
                }
            }
        }

        private void OnValidate() {
            Animator = GetComponentInChildren<AnimationAdapter>();
            Movement = GetComponentInChildren<MovementAdapter>();
            Detector = GetComponentInChildren<DetectorAdapter>();
            CheckInjectorsOnSameObject();
        }

        private void Awake() {
            Init();
        }

        ///<summary>Initialises states as dependency tree</summary>
        private void Init() {
            int i = 0;

            StateMachine = new StateMachine();
            StateFactory factory = new StateFactory(StateMachine, this);
            StateMachine.SetRoot(factory.Create(AIState.Root, null));

            foreach (AIStateView view in _states) {
                _lookup.Add(view.Key, i);
                i++;
            }
            Dictionary<AIState, int> visited = new Dictionary<AIState, int>();

            foreach (AIStateView view in _states) {
                if (view.Key == AIState.Root) { continue; }
                InitState(view, factory, visited);
            }

            if (_definition != null) {
                _definition.InitInjectors(this);
                _definition.InitTransitions(this);
            } else {
                Helpers.ContextLog(this, "State machine state defitions are null, this is not valid, please select an option from the drop down...");
                enabled = false;
                return;
            }
        }

        ///<summary>Initialises view.State with real state from factory resolving parents first</summary>
        ///<remarks>Root state should already be initialised</remarks>
        private void InitState(AIStateView view, StateFactory factory, Dictionary<AIState, int> visited) {
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
                InitState(_states[_lookup[view.Parent]], factory, visited);
            }
            if (visited.ContainsKey(view.Key)) {
                visited[view.Key]++;
            } else {
                visited.Add(view.Key, 1);
            }
            this[view.Key] = factory.Create(view.Key, this[view.Parent]);
        }

        ///<summary>Provides all transitions to state machine after injectors are initialised</summary>
        ///<summary>Use IStateDefinitionOverride to override this behaviour</summary>
        private void DefaultTransitions() {
        }

        ///<summary>Updates state machine</summary>
        ///<summary>Unlikely to change unless state machine needs to not tick on Update</summary>
        private void Update() {
            float dt = Time.deltaTime;
            CooldownManager.Update(dt);
            StateMachine.Tick(dt);
            _statePath = string.Join(" > ", StateMachine.Root.GetLeaf().PathToRoot().Reverse());
        }

        ///<summary>Forwards FixedUpdate to state machine</summary>
        private void FixedUpdate() {
            StateMachine.FixedTick();
        }
    }

    public interface IStateDefinition {
        public void InitInjectors(StateMachineContext context);
        public void InitTransitions(StateMachineContext context);
    }

    [Serializable]
    public class DefaultStateDefinitions : IStateDefinition {
        public void InitInjectors(StateMachineContext context) {
            context.IdleInjector = context.GetComponentInChildren<IIdleInjector>();
            context.WanderInjector = context.GetComponentInChildren<IWanderInjector>();
            context.PatrolInjector = context.GetComponentInChildren<IPatrolInjector>();
            context.ChaseInjector = context.GetComponentInChildren<IChaseInjector>();
            context.AttackInjector = context.GetComponentInChildren<IAttackInjector>();
            context.IdleInjector.Init();
            context.WanderInjector.Init();
            context.PatrolInjector.Init();
            context.ChaseInjector.Init();
            context.AttackInjector.Init();
        }

        public void InitTransitions(StateMachineContext context) {
            context.StateMachine.AddInitialState(context[AIState.Root], context[AIState.Idle]);
            // Idle
            context.StateMachine.AddStateTransition(
                context[AIState.Idle],
                context[AIState.Wander],
                new AndPredicate(new LambdaPredicate(() => context.IdleInjector.DoneIdling(context)), new RandomChancePredicate(0.5f)));

            context.StateMachine.AddStateTransition(
                context[AIState.Idle],
                context[AIState.Patrol],
                new AndPredicate(new LambdaPredicate(() => context.IdleInjector.DoneIdling(context)), new RandomChancePredicate(0.5f)));

            context.StateMachine.AddStateTransition(
                context[AIState.Idle],
                context[AIState.Chase],
                new LambdaPredicate(context.Detector.HasTarget));

            // Wander
            context.StateMachine.AddStateTransition(
                context[AIState.Wander],
                context[AIState.Chase],
                new LambdaPredicate(context.Detector.HasTarget));

            // Patrol
            context.StateMachine.AddStateTransition(
                context[AIState.Patrol],
                context[AIState.Chase],
                new LambdaPredicate(context.Detector.HasTarget));

            // Chase
            context.StateMachine.AddStateTransition(
                context[AIState.Chase],
                context[AIState.Attack],
                new LambdaPredicate(() => context.ChaseInjector.InAttackRange(context)));

            context.StateMachine.AddStateTransition(
                context[AIState.Chase],
                context[AIState.Idle],
                new LambdaPredicate(() => context.ChaseInjector.LostTarget(context)));

            // Attack
            context.StateMachine.AddStateTransition(
                context[AIState.Attack],
                context[AIState.Chase],
                new LambdaPredicate(() => context.AttackInjector.UnableToAttack(context)));

        }
    }
}
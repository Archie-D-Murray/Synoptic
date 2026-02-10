using System.Collections.Generic;
using System.Reflection;

namespace AI.HSM {
    public class StateMachine {
        public readonly State Root;
        private bool _started = false;
        public readonly Dictionary<State, State> InitialStates = new Dictionary<State, State>();
        public readonly Dictionary<State, HashSet<TransitionCondition>> StateTransitions = new Dictionary<State, HashSet<TransitionCondition>>();
        public readonly HashSet<TransitionCondition> AnyStateTransition = new HashSet<TransitionCondition>();

        public StateMachine(State root) {
            Root = root;
        }

        public void Start() {
            if (_started) { return; }
            _started = true;
            Root.Enter();
        }

        public void Tick(float deltaTime) {
            if (!_started) { Start(); }
            Root.Update(deltaTime);
        }

        public void FixedTick() {
            Root.FixedUpdate();
        }

        public void ChangeState(State from, State to) {
            if (from == to || from == null || to == null) { return; }
            State lca = State.LCA(from, to);
            for (State state = from; state != lca; state = state.Parent) { state.Exit(); }
            Stack<State> toStack = new Stack<State>();
            for (State state = to; state != lca; state = state.Parent) { toStack.Push(state); }
            while (toStack.Count > 0) { toStack.Pop().Enter(); }
        }

        public void AddAnyTransition(State to, IPredicate condition) {
            AnyStateTransition.Add(new TransitionCondition(to, condition));
        }

        public void AddStateTransition(State from, State to, IPredicate condition) {
            AddState(from, new TransitionCondition(to, condition));
        }

        private void AddState(State from, TransitionCondition condition) {
            if (StateTransitions.TryGetValue(from, out HashSet<TransitionCondition> transitions)) {
                transitions.Add(condition);
            } else {
                StateTransitions.Add(from, new HashSet<TransitionCondition>() { condition });
            }
        }
    }

    public class StateMachineBuilder {
        private readonly State _root;

        public StateMachineBuilder(State root) {
            _root = root;
        }

        public StateMachine Build() {
            StateMachine stateMachine = new StateMachine(_root);
            Wire(_root, stateMachine, new HashSet<State>());
            return stateMachine;
        }

        // This is gross but it works and anything else would be a massive pain - it essentially assigns the state machine to all states
        private void Wire(State state, StateMachine machine, HashSet<State> visited) {
            if (state == null || !visited.Add(state)) { return; }

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            FieldInfo machineField = typeof(State).GetField(nameof(State.StateMachine), flags);
            if (machineField != null) { machineField.SetValue(state, machine); }

            foreach (FieldInfo field in state.GetType().GetFields(flags)) { // Get all fields of type AIState
                if (!typeof(State).IsAssignableFrom(field.FieldType)) { continue; }
                if (field.Name == nameof(State.Parent)) { continue; } // No infinite recursion
                State child = (State) field.GetValue(state);
                if (child == null) { continue; }
                if (!ReferenceEquals(child.Parent, state)) { continue; }
                Wire(child, machine, visited); // Recurse over children
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.HSM {
    [Serializable]
    public class State {
        public readonly StateMachine StateMachine;
        public readonly State Parent;
        public State ActiveChild;
        public float cost = 0;

        protected State(StateMachine stateMachine, State parent) {
            StateMachine = stateMachine;
            Parent = parent;
        }

        protected virtual State GetInitialState() { return null; }
        protected virtual State GetTransition() { return null; }

        protected virtual void OnEnter() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnUpdate(float dt) { }
        protected virtual void OnExit() { }

        public void Enter() {
            if (Parent != null) { Parent.ActiveChild = this; }
            OnEnter();
            State initial = GetInitialState();
            if (initial != null) { initial.Enter(); }
        }

        public void Update(float deltaTime) {
            State transition = GetTransition();
            if (transition != null) {
                StateMachine.ChangeState(this, transition);
                return;
            }

            if (ActiveChild != null) { ActiveChild.Update(deltaTime); }
            OnUpdate(deltaTime);
        }

        public void FixedUpdate() {
            if (ActiveChild != null) { ActiveChild.FixedUpdate(); }
            OnFixedUpdate();
        }

        public void Exit() {
            if (ActiveChild != null) { ActiveChild.Exit(); } // Depth first out
            ActiveChild = null;
            OnExit();
        }

        public State GetLeaf() {
            State leaf = this;
            while (leaf.ActiveChild != null) { leaf = leaf.ActiveChild; }
            return leaf;
        }

        ///<summary>Find path to root state from current using parent</summary>
        ///<returns>List of states in order self -> parent -> ... -> root</returns>
        public IEnumerable<State> PathToRoot() {
            for (State current = this; current != null; current = current.Parent) {
                yield return current;
            }
        }

        public static string StatePath(State state) {
            return string.Join(" > ", state.PathToRoot().Reverse().Select(state => state.GetType().Name));
        }

        public T Cast<T>() where T : State {
            return this as T;
        }

        public static State LCA(State a, State b) {
            HashSet<State> aParents = new HashSet<State>();
            for (State s = a; s != null; s = s.Parent) { aParents.Add(s); }
            for (State s = b; s != null; s = s.Parent) {
                if (aParents.Contains(s)) { return s; } // Parent of a and b
            }

            return null;
        }
    }
}
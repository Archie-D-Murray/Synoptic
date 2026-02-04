using System;
using System.Collections.Generic;

using AI.Adapters;

using UnityEngine;

namespace AI.HSM {

    [Serializable]
    public class AIStateView {
        public AIStates Key;
        public AIStates Parent;
        [HideInInspector]
        public State State = null;
    }

    class StateNode {
        public StateNode Parent;
        public State Value;
        public AIStates Type;
        public List<StateNode> Children;
    }

    [Serializable]
    public class StateMachineContext {
        public MovementAdapter Adapter;
        public StateMachine StateMachine;

        [SerializeField] private AIStateView[] _array;
        private Dictionary<AIStates, int> _lookup = new Dictionary<AIStates, int>();

        public State this[AIStates state] {
            get {
                if (_lookup.TryGetValue(state, out int index)) {
                    return _array[index].State;
                } else {
                    return null;
                }
            }
            private set {
                if (_lookup.TryGetValue(state, out int index)) {
                    _array[index].State = value;
                }
            }
        }

        class StateNode {
            AIStates State;
            AIStates Parent;
            List<AIStates> Children;
        }
        public void Init() {


            StateBuilder builder = new StateBuilder();
            int index = 0;
            foreach (AIStateView view in _array) {
                _lookup.Add(view.Key, index);
                index++;
            }

            foreach (AIStateView view in _array) {
                if (view.Parent == AIStates.None) {
                    view.State = builder.Create(AIStates.None, null);
                }
            }
        }
    }
}
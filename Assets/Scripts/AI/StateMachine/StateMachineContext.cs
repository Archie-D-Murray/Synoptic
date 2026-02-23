using System;
using System.Collections.Generic;
using System.Linq;

using AI.Adapters;

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
        public StateMachine StateMachine;
        public Transform Self;

        [SerializeField] private AIStateView[] _array;
        private Dictionary<AIState, int> _lookup = new Dictionary<AIState, int>();

        public State this[AIState state] {
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
            AIState State;
            AIState Parent;
            List<AIState> Children;
        }

        public void Init() {
            StateFactory factory = new StateFactory(StateMachine, this);
            int index = 0;
            foreach (AIStateView view in _array) {
                _lookup.Add(view.Key, index);
                index++;
            }

            // TODO: Use dependency resolving to handle this as a tree
        }

        private void GetDefaultTransitions() {

        }
    }
}
using System;
using System.Collections.Generic;

using AI.Adapters;

using UnityEngine;

using Utilities;

namespace AI.HSM {

    [Serializable]
    public class AIStateStatePair : SerializedPairBase {
        public AIStates Key;
        public State Value;
    }

    [Serializable]
    public class StatesLookup {
        [SerializeField] private AIStateStatePair[] _array;
        private Dictionary<AIStates, int> _lookup = new Dictionary<AIStates, int>();

        public State this[AIStates state] {
            get {
                if (_lookup.TryGetValue(state, out int index)) {
                    return _array[index].Value;
                } else {
                    return null;
                }
            }
            set {
                if (_lookup.TryGetValue(state, out int index)) {
                    _array[index].Value = value;
                }
            }
        }
    }

    [Serializable]
    public class StateMachineContext {
        public MovementAdapter Adapter;
        public StatesLookup States;
    }
}
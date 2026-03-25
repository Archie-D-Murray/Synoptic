using System;
using System.Linq;

using UnityEngine;

namespace AI.Adapters {
    public abstract class DetectorAdapter : MonoBehaviour {

        protected Transform _target = null;
        protected Vector3 _lastTargetPosition;
        protected bool _justLostTarget;

        protected abstract Transform FindTarget();

        public Transform Target => _target;
        public Vector3 TargetPosition => _target ? _target.position : _lastTargetPosition;
        public bool JustLostTarget => _justLostTarget;
        public bool HasTarget() {
            return Target;
        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Utilities;

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

    public class RangeDetectorAdapter : DetectorAdapter {

        [SerializeField] private Vector3 _offset = Vector3.up * 1.5f;
        [SerializeField] private float _range = 5.0f;
        [SerializeField] private LayerMask _mask = int.MaxValue;
        [SerializeField] private CountDownTimer _searchTimer = new CountDownTimer(0.1f);

        protected Collider[] _found = new Collider[32];
        protected HashSet<Transform> _unique = new HashSet<Transform>(32);

        protected override Transform FindTarget() {
            _justLostTarget = false;

            if (_target && Vector3.SqrMagnitude(_target.position - transform.position) <= _range) {
                _lastTargetPosition = _target.position;
                return _target;
            }

            int count = Physics.OverlapSphereNonAlloc(transform.position + _offset, _range, _found, _mask);

            float dist = float.MaxValue;
            Transform closest = null;
            for (int i = 0; i < count; i++) {
                if (_unique.Add(_found[i].transform)) {
                    float distance = (_found[i].transform.position - transform.position).sqrMagnitude;
                    if (dist > distance) {
                        dist = distance;
                        closest = _found[i].transform;
                    }
                }
            }

            if (_unique.Contains(_target)) {
                _lastTargetPosition = _target.position;
                return _target;
            } else if (closest) {
                _lastTargetPosition = closest.position;
                return closest;
            } else {
                _justLostTarget = true;
                return null;
            }
        }

        protected void FixedUpdate() {
            _searchTimer.Update(Time.fixedDeltaTime);
            if (_searchTimer.IsFinished) {
                _target = FindTarget();
                _searchTimer.Reset();
            }
        }
    }
}
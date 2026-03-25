using System.Collections.Generic;

using UnityEngine;

using Utilities;

namespace AI.Adapters {
    public class RangeDetectorAdapter : DetectorAdapter {

        [SerializeField] private Vector3 _offset = Vector3.up * 1.5f;
        [SerializeField] private float _range = 5.0f;
        [SerializeField] private LayerMask _mask = int.MaxValue;
        [SerializeField] private CountDownTimer _searchTimer = new CountDownTimer(0.1f);

        protected Collider[] _found = new Collider[32];
        protected HashSet<Transform> _unique = new HashSet<Transform>(32);

        protected override Transform FindTarget() {
            _justLostTarget = false;

            if (_target && Vector3.SqrMagnitude(_target.position - transform.position) <= _range * _range) {
                _lastTargetPosition = _target.position;
                return _target;
            } else if (_target) {
                Helpers.ContextLog(this, "Lost target previous");
                _target = null;
                _justLostTarget = true;
            }

            Transform closest = null;
            if (!_target) {
                int count = Physics.OverlapSphereNonAlloc(transform.position + _offset, _range, _found, _mask);

                float dist = float.MaxValue;
                for (int i = 0; i < count; i++) {
                    if (_found[i].transform.ContainsParentInHierarchy(transform.root)) { continue; }
                    if (_unique.Add(_found[i].transform)) {
                        float distance = (_found[i].transform.position - transform.position).sqrMagnitude;
                        if (dist > distance) {
                            dist = distance;
                            closest = _found[i].transform;
                        }
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
                Helpers.ContextLog(this, "Lost target");
                _target = null;
                _justLostTarget = true;
                return null;
            }
        }

        protected void Awake() {
            _searchTimer.Start();
        }

        protected void FixedUpdate() {
            _searchTimer.Update(Time.fixedDeltaTime);
            if (_searchTimer.IsFinished) {
                _target = FindTarget();
                _searchTimer.Reset();
                _searchTimer.Start();
            }
        }
    }
}
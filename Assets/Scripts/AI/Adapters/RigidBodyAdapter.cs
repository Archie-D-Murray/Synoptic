using UnityEngine;

namespace AI.Adapters {
    public class RigidBodyAdapter : MovementAdapter {
        private Rigidbody _rb;

        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _targetSpeed;

        [SerializeField] private Vector3 _target;
        [SerializeField] private bool _hasTarget = false;

        public override Vector3 Target => _target;

        private void OnValidate() {
            _rb = GetComponent<Rigidbody>();
        }

        public override void Move(Vector3 velocity) {
            _hasTarget = false;
            _rb.linearVelocity = velocity;
        }

        public override void SetDestination(Vector3 targetPosition) {
            _hasTarget = true;
            _target = targetPosition;
        }

        private void FixedUpdate() {
            if (_hasTarget) {
                _targetSpeed = Mathf.Clamp01(Vector3.Distance(_target, _rb.position) / _maxSpeed);
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, Time.fixedDeltaTime * _acceleration);
                _rb.linearVelocity = (_target - _rb.position).normalized * _currentSpeed;
            } else {
                _currentSpeed = _rb.linearVelocity.magnitude;
            }
        }

        public override float Speed() {
            return _rb.linearVelocity.magnitude;
        }

        public override Vector3 Velocity() {
            return _rb.linearVelocity;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool Equals(object other) {
            return base.Equals(other);
        }

        public override string ToString() {
            return base.ToString();
        }
    }
}
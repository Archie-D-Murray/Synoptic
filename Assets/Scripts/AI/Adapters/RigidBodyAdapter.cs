using UnityEngine;

namespace AI.Adapters {
    public class RigidBodyAdapter : MovementAdapter {
        [SerializeField] private Rigidbody _rb;

        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _targetSpeed;

        [SerializeField] private Vector3 _target;
        [SerializeField] private bool _hasTarget = false;

        ///<summary>Target position to navigate to</summary>
        public override Vector3 Target => _target;

        private void OnValidate() {
            _rb = GetComponent<Rigidbody>();
        }

        ///<summary>Move using velocity</summary>
        ///<param name="velocity">Velocity to move with (not multiplied by either Time.fixedDeltaTime or Time.deltaTime)</param>
        public override void Move(Vector3 velocity) {
            _hasTarget = false;
            _rb.linearVelocity = velocity;
        }

        ///<summary>Start moving toward destination (without pathfinding)</summary>
        ///<param name="targetPosition">Position to move to - used as delta to provide direction</param>
        public override void SetDestination(Vector3 targetPosition) {
            _hasTarget = true;
            _target = targetPosition;
        }

        private void FixedUpdate() {
            if (_hasTarget) { // Moving in direction of target
                _targetSpeed = Mathf.Min(Vector3.Distance(_target, _rb.position) * _maxSpeed, _maxSpeed);
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, Time.fixedDeltaTime * _acceleration);
                _rb.linearVelocity = Vector3.MoveTowards(Vector3.zero, _target - _rb.position, _currentSpeed);
            } else {
                _currentSpeed = _rb.linearVelocity.magnitude;
            }
        }

        ///<summary>Current speed</summary>
        public override float Speed() {
            return _rb.linearVelocity.magnitude;
        }

        ///<summary>Current velocity</summary>
        public override Vector3 Velocity() {
            return _rb.linearVelocity;
        }
    }
}
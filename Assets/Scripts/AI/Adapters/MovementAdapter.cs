using UnityEngine;

using UnityEngine.AI;

namespace AI.Adapters {

    public abstract class MovementAdapter : MonoBehaviour {
        protected float _maxSpeed;
        protected float _acceleration;

        public abstract void SetDestination(Vector3 targetPosition);
        public abstract void Move(Vector3 velocity);

        public abstract float Speed();
        public abstract Vector3 Velocity();
    }

    public class RigidBodyAdapter : MovementAdapter {
        private Rigidbody _rb;

        private float _currentSpeed;
        private float _targetSpeed;

        private Vector3 _target;
        private bool _hasTarget = false;

        public override void Move(Vector3 velocity) {
            _hasTarget = false;
            _rb.linearVelocity = velocity;
        }

        public override void SetDestination(Vector3 targetPosition) {
            _hasTarget = true;
            _target = targetPosition;
        }

        private void Update(float dt) {
            if (_hasTarget) {
                _targetSpeed = Mathf.Clamp01(Vector3.Distance(_target, _rb.position) / _maxSpeed);
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, dt * _acceleration);
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
    }

    public class NavmeshAdapter : MovementAdapter {
        private NavMeshAgent _agent;
        private NavMeshHit _hit;
        private float _maxDistance = 1.0f;

        public override void Move(Vector3 velocity) {
            _agent.velocity = velocity;
        }

        public override void SetDestination(Vector3 targetPosition) {
            if (NavMesh.SamplePosition(targetPosition, out _hit, _maxDistance, NavMesh.AllAreas)) {
                _agent.SetDestination(_hit.position);
            }
        }

        public override float Speed() {
            return _agent.velocity.magnitude;
        }

        public override Vector3 Velocity() {
            return _agent.velocity;
        }
    }
}
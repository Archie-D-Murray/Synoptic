using UnityEngine;

using UnityEngine.AI;

namespace AI.Adapters {
    public class NavmeshAdapter : MovementAdapter {
        private NavMeshAgent _agent;
        private NavMeshHit _hit;
        private float _maxDistance = 1.0f;

        public override Vector3 Target => _agent.destination;

        public override bool Equals(object other) {
            return base.Equals(other);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

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

        public override string ToString() {
            return base.ToString();
        }

        public override Vector3 Velocity() {
            return _agent.velocity;
        }
    }
}
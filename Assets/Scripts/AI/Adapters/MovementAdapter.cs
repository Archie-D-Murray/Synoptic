using UnityEngine;

namespace AI.Adapters {

    public abstract class MovementAdapter : MonoBehaviour {
        [SerializeField] protected float _maxSpeed = 1.0f;
        [SerializeField] protected float _acceleration = 1.0f;

        public abstract Vector3 Target { get; }

        public abstract void SetDestination(Vector3 targetPosition);
        public abstract void Move(Vector3 velocity);

        public abstract float Speed();
        public abstract Vector3 Velocity();
    }
}
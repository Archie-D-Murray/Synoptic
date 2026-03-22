using UnityEngine;

namespace AI.Adapters {

    public abstract class MovementAdapter : MonoBehaviour {
        protected float _maxSpeed;
        protected float _acceleration;

        public abstract void SetDestination(Vector3 targetPosition);
        public abstract void Move(Vector3 velocity);

        public abstract float Speed();
        public abstract Vector3 Velocity();
    }
}
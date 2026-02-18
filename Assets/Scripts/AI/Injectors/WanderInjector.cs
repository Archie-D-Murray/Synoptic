using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class WanderInjector : IWanderInjector {

        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private float _maxRange = 10.0f;

        private StateMachineContext _context;

        public void ProvideState(StateMachineContext context) {
            _context = context;
            _initialPosition = _context.Movement.transform.position;
        }

        public Vector3 GetWanderPoint() {
            return _initialPosition + (Random.insideUnitCircle * _maxRange).ToXZ();
        }

        public void OnEnter() {
            _context.Movement.SetDestination(GetWanderPoint());
        }

        public void OnExit() { }

        public void OnUpdate(float dt) { }
    }
}
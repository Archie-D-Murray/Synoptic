using AI.HSM;

using UnityEngine;

using Utilities;

namespace AI.Injectors {
    public class WanderInjector : IWanderInjector {

        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private Vector3 _wanderTarget;
        [SerializeField] private float _maxRange = 10.0f;
        [SerializeField] private float _minWanderTime = 5.0f;
        [SerializeField] private float _maxWanderTime = 10.0f;
        [SerializeField] private CountDownTimer _wanderTimer = new CountDownTimer(5.0f);

        private StateMachineContext _context;

        public void ProvideState(StateMachineContext context) {
            _context = context;
            _initialPosition = _context.Self.position;
        }

        public Vector3 GetWanderPoint() {
            _wanderTarget = _initialPosition + (Random.insideUnitCircle * _maxRange).ToXZ();
            _wanderTimer.Reset(Random.Range(_minWanderTime, _maxWanderTime));
            return _wanderTarget;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) {
            if (Vector3.Distance(_wanderTarget, _context.Self.position) <= 0.5f && !_wanderTimer.IsRunning) {
                _wanderTimer.Update(dt);
            }
        }

        public bool NextWanderPoint() {
            return _wanderTimer.IsFinished;
        }
    }
}
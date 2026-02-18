using AI.HSM;

using UnityEngine;

using Utilities;

namespace AI.Injectors {

    public class RandomIdleInjector : MonoBehaviour, IIdleInjector {
        [SerializeField] private CountDownTimer _idleTimer;
        [SerializeField] private float _maxWaitTime = 15;
        [SerializeField] private float _minWaitTime = 5;

        private StateMachineContext _context;

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }

        private void Start() {
            _idleTimer = new CountDownTimer(Random.Range(_minWaitTime, _maxWaitTime));
        }

        public void OnEnter() {
            _idleTimer.Reset(Random.Range(_minWaitTime, _maxWaitTime));
            _idleTimer.Start();
        }

        public bool DoneIdling() {
            return _idleTimer.IsFinished;
        }

        public void OnUpdate(float dt) {
            _idleTimer.Update(dt);
        }

        public void OnExit() { }
    }
}
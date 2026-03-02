using AI.HSM;

using UnityEngine;

using Utilities;

namespace AI.Injectors {
    public class ChaseInjector : MonoBehaviour, IChaseInjector {
        [SerializeField] private float _chaseRange;
        [SerializeField] private float _attackRange;
        [SerializeField] private CountDownTimer _lostTargetTimer = new CountDownTimer(1.5f);

        private StateMachineContext _context;

        public bool LostTarget() {
            return !_context.Detector.Target && _lostTargetTimer.IsFinished;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) {
            _lostTargetTimer.Update(dt);
        }

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }

        public void StartLostTimer() {
            _lostTargetTimer.Start();
        }

        public bool SwitchToAttack() {
            return (_context.Detector.TargetPosition - transform.position).sqrMagnitude <= _attackRange * _attackRange;
        }
    }
}
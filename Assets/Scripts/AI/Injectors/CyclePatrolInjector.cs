using AI.HSM;

using UnityEngine;

using Utilities;

namespace AI.Injectors {
    public class CyclePatrolInjector : MonoBehaviour, IPatrolInjector {
        [SerializeField] private Vector3[] _wanderPoints;
        [SerializeField] private int _patrolIndex = 0;
        [SerializeField] private CountDownTimer _timePerPoint = new CountDownTimer(2.5f);
        [SerializeField] private float _targetDistance = 0.5f;
        private StateMachineContext _context;

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }

        public Vector3 GetPatrolTarget() {
            return _wanderPoints[_patrolIndex];
        }

        public void OnEnter() {
        }

        public void OnExit() { }

        public void OnUpdate(float dt) {
            if (!_timePerPoint.IsRunning && CloseToTarget(_patrolIndex, _targetDistance)) {
                _timePerPoint.Start();
            }

            if (_timePerPoint.IsFinished) {
                Next();
            }
        }

        public void Next() {
            _patrolIndex = ++_patrolIndex % _wanderPoints.Length;
        }

        public void Prev() {
            if (_patrolIndex > 1) {
                _patrolIndex--;
            } else {
                _patrolIndex = _wanderPoints.Length - 1;
            }
        }

        private bool CloseToTarget(int current, float maxDistance) {
            return Vector3.Distance(_context.Movement.transform.position, GetPatrolTarget()) <= maxDistance;
        }
    }
}
using AI.HSM;

using UnityEngine;

using Utilities;

namespace AI.Injectors {
    public class CyclePatrolInjector : MonoBehaviour, IPatrolInjector {
        [SerializeField] private Vector3[] _wanderPoints;
        [SerializeField] private CountDownTimer _timePerPoint = new CountDownTimer(2.5f);
        [SerializeField] private float _targetDistance = 0.5f;
        [SerializeField] private int _patrolIndex;
        private StateMachineContext _context;

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }

        public Vector3 GetPatrolTarget(int index) {
            return _wanderPoints[index];
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) { }

        public int Next(int index) {
            return ++index % _wanderPoints.Length;
        }

        public int Prev(int index) {
            if (index > 1) {
                return index - 1;
            } else {
                return _wanderPoints.Length - 1;
            }
        }

        public void TickPatrolPoint(float dt) {
            if (!_timePerPoint.IsRunning) { _timePerPoint.Start(); }
            _timePerPoint.Update(dt);
        }

        public bool FinishedPatrolPoint(int index) {
            return _timePerPoint.IsFinished;
        }

        public bool AtPatrolPoint(Vector3 position, int index) {
            return Vector3.Distance(position, GetPatrolTarget(index)) <= _targetDistance;
        }
    }
}
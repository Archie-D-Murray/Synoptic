using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class PatrolInjector : MonoBehaviour, IPatrolInjector {
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private int _patrolIndex = 0;
        private StateMachineContext _context;

        public Vector3 GetPatrolTarget() {
            return _patrolPoints[_patrolIndex].position;
        }

        public void Next() {
            _patrolIndex = ++_patrolIndex % _patrolPoints.Length;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) { }

        public void Prev() {
            if (_patrolIndex > 0) {
                _patrolIndex--;
            } else {
                _patrolIndex = _patrolPoints.Length - 1;
            }
        }

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }
    }
}
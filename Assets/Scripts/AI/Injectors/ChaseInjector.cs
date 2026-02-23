using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class ChaseInjector : MonoBehaviour, IChaseInjector {
        [SerializeField] private Transform _player;
        [SerializeField] private float _chaseRange;
        [SerializeField] private float _attackRange;

        private StateMachineContext _context;

        private Vector3 _playerDelta => _player ? _player.position - _context.Self.position : Vector3.positiveInfinity;

        public bool LostTarget() {
            return !_playerDelta.InRange(_chaseRange);
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) { }

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }

        public bool SwitchToAttack() {
            return _playerDelta.InRange(_attackRange);
        }
    }
}
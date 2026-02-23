using System.Collections.Generic;

using AI.Adapters;
using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class AttackInjector : MonoBehaviour, IAttackInjector {
        [SerializeField] private float _attackCooldown = 1.0f;
        private StateMachineContext _context;
        [SerializeField] private NullAttackAdapter _nullAttack = new NullAttackAdapter();

        public List<AttackAdapter> GetAttacks() {
            return new List<AttackAdapter>() { _nullAttack };
        }

        public float GetAttackCooldown() {
            return _attackCooldown;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) { }

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }
    }
}
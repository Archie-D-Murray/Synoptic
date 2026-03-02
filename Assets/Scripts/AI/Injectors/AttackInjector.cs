using System.Collections.Generic;

using AI.Adapters;
using AI.HSM;

using UnityEngine;

using Utilities;

namespace AI.Injectors {
    public class AttackInjector : MonoBehaviour, IAttackInjector {
        [SerializeField] private CountDownTimer _attackTimer = new CountDownTimer(1.0f);
        private StateMachineContext _context;
        [SerializeField] private NullAttackAdapter _nullAttack = new NullAttackAdapter();

        public List<AttackAdapter> GetAttacks() {
            return new List<AttackAdapter>() { _nullAttack };
        }

        public bool CanAttack() {
            return _attackTimer.IsFinished;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void OnUpdate(float dt) {
            _attackTimer.Update(dt);
        }

        public void ProvideState(StateMachineContext context) {
            _context = context;
        }

        public float AttackTime() {
            return _attackTimer.InitialTime;
        }

        public void RestartAttackCooldown() {
            _attackTimer.Reset();
            _attackTimer.Start();
        }
    }
}
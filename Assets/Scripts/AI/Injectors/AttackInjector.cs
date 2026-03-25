using System.Collections.Generic;

using AI.Adapters;
using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class AttackInjector : MonoBehaviour, IAttackInjector {
        [SerializeField] private List<AttackAdapter> _attacks = new List<AttackAdapter>() { new NullAttackAdapter() };
        [SerializeField] private float _attackRange;

        private static int _attackCD = AICooldownManager.GetHash("Attack");

        public List<AttackAdapter> GetAttacks(StateMachineContext context) {
            return _attacks;
        }

        public bool CanAttack(StateMachineContext context) {
            return context.Cooldowns.Get(_attackCD).IsFinished;
        }

        public void OnEnter(StateMachineContext context) { }

        public void OnExit(StateMachineContext context) { }

        public void OnUpdate(StateMachineContext context, float dt) {
            context.Cooldowns.Get(_attackCD).Update(dt);
        }

        public void Init() { }

        public float AttackTime(StateMachineContext context) {
            return context.Cooldowns.Get(_attackCD).InitialTime;
        }

        public void RestartAttackCooldown(StateMachineContext context) {
            context.Cooldowns.Get(_attackCD).Reset();
            context.Cooldowns.Get(_attackCD).Start();
        }

        public bool SwitchToChase(StateMachineContext context) {
            return Vector3.Distance(context.Detector.TargetPosition, context.Position) >= _attackRange;
        }
    }
}
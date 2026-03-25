using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class ChaseInjector : MonoBehaviour, IChaseInjector {
        [SerializeField] private float _chaseRange;
        [SerializeField] private float _attackRange;

        private static int _lostTargetID = AICooldownManager.GetHash("LostTargetTimer");

        public bool LostTarget(StateMachineContext context) {
            return !context.Detector.Target && context.CooldownManager.Get(_lostTargetID).IsFinished;
        }

        public void OnEnter(StateMachineContext context) { }

        public void OnExit(StateMachineContext context) { }

        public void OnUpdate(StateMachineContext context, float dt) {
            context.CooldownManager.Get(_lostTargetID).Update(dt);
        }

        public void Init() { }

        public void StartLostTimer(StateMachineContext context) {
            context.CooldownManager.Get(_lostTargetID).Start();
        }

        public bool SwitchToAttack(StateMachineContext context) {
            return (context.Detector.TargetPosition - context.Position).sqrMagnitude <= _attackRange * _attackRange;
        }
    }
}
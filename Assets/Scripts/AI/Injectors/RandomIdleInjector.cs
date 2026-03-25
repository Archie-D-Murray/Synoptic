using AI.HSM;

using UnityEngine;

namespace AI.Injectors {

    public class RandomIdleInjector : MonoBehaviour, IIdleInjector {
        [SerializeField] private float _maxWaitTime = 15;
        [SerializeField] private float _minWaitTime = 5;

        private static int _waitTimeID = AICooldownManager.GetHash("WaitTime");

        public void Init() { }

        private void Start(StateMachineContext context) {
            context.Cooldowns.Get(_waitTimeID).Reset(Random.Range(_minWaitTime, _maxWaitTime));
        }

        public void OnEnter(StateMachineContext context) {
            context.Cooldowns.Get(_waitTimeID).Reset(Random.Range(_minWaitTime, _maxWaitTime));
            context.Cooldowns.Get(_waitTimeID).Start();
        }

        public bool DoneIdling(StateMachineContext context) {
            return context.Cooldowns.Get(_waitTimeID).IsFinished;
        }

        public void OnUpdate(StateMachineContext context, float dt) {
            context.Cooldowns.Get(_waitTimeID).Update(dt);
        }

        public void OnExit(StateMachineContext context) { }
    }
}
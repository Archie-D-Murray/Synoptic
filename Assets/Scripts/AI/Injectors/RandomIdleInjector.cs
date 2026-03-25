using AI.HSM;

using UnityEngine;

namespace AI.Injectors {

    public class RandomIdleInjector : MonoBehaviour, IIdleInjector {
        [SerializeField] private float _maxWaitTime = 15;
        [SerializeField] private float _minWaitTime = 5;

        private static int _waitTimeID = AICooldownManager.GetHash("WaitTime");

        public void Init() { }

        public void OnEnter(StateMachineContext context) {
            context.CooldownManager.Get(_waitTimeID).Reset(Random.Range(_minWaitTime, _maxWaitTime));
            context.CooldownManager.Get(_waitTimeID).Start();
        }

        public bool DoneIdling(StateMachineContext context) {
            return context.CooldownManager.Get(_waitTimeID).IsFinished;
        }

        public void OnUpdate(StateMachineContext context, float dt) {
            context.CooldownManager.Get(_waitTimeID).Update(dt);
        }

        public void OnExit(StateMachineContext context) { }
    }
}
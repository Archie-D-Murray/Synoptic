using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class WanderInjector : MonoBehaviour, IWanderInjector {

        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private float _maxRange = 10.0f;
        [SerializeField] private float _minWanderTime = 5.0f;
        [SerializeField] private float _maxWanderTime = 10.0f;

        private int _wanderTimerID = AICooldownManager.GetHash("WanderTimer");

        public void Init() {
            _initialPosition = transform.position;
        }

        public Vector3 GetWanderPoint(StateMachineContext context) {
            context.CooldownManager.Get(_wanderTimerID).Reset(Random.Range(_minWanderTime, _maxWanderTime));
            return _initialPosition + (Random.insideUnitCircle * _maxRange).ToXZ();
        }

        public void OnEnter(StateMachineContext context) { }

        public void OnExit(StateMachineContext context) { }

        public void OnUpdate(StateMachineContext context, float dt) {
            if (Vector3.Distance(context.Movement.Target, context.Position) <= 0.5f && !context.CooldownManager.Get(_wanderTimerID).IsRunning) {
                context.CooldownManager.Get(_wanderTimerID).Update(dt);
            }
        }

        public bool NextWanderPoint(StateMachineContext context) {
            return context.CooldownManager.Get(_wanderTimerID).IsFinished;
        }
    }
}
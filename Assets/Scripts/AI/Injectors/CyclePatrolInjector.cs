using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public class CyclePatrolInjector : MonoBehaviour, IPatrolInjector {
        [SerializeField] private Vector3[] _wanderPoints;
        [SerializeField] private float _targetDistance = 0.5f;

        private static int _timePerPointID = AICooldownManager.GetHash("TimePerPoint");

        public void Init() { }

        public Vector3 GetPatrolTarget(StateMachineContext context, int index) {
            return _wanderPoints[index];
        }

        public void OnEnter(StateMachineContext context) { }

        public void OnExit(StateMachineContext context) { }

        public void OnUpdate(StateMachineContext context, float dt) { }

        public int Next(StateMachineContext context, int index) {
            return ++index % _wanderPoints.Length;
        }

        public int Prev(StateMachineContext context, int index) {
            if (index > 1) {
                return index - 1;
            } else {
                return _wanderPoints.Length - 1;
            }
        }

        public void TickPatrolPoint(StateMachineContext context, float dt) {
            if (!context.CooldownManager.Get(_timePerPointID).IsRunning) { context.CooldownManager.Get(_timePerPointID).Start(); }
            context.CooldownManager.Get(_timePerPointID).Update(dt);
        }

        public bool FinishedPatrolPoint(StateMachineContext context, int index) {
            return context.CooldownManager.Get(_timePerPointID).IsFinished;
        }

        public bool AtPatrolPoint(StateMachineContext context, Vector3 position, int index) {
            return Vector3.Distance(position, GetPatrolTarget(context, index)) <= _targetDistance;
        }
    }
}
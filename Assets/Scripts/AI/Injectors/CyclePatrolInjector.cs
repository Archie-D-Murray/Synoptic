using AI.HSM;

using UnityEngine;

namespace AI.Injectors {

    ///<summary>Moves between points idling for a time at each point</summary>
    public class CyclePatrolInjector : MonoBehaviour, IPatrolInjector {

        ///<summary>Patrol points</summary>
        [SerializeField] private Vector3[] _patrolPoints;

        ///<summary>Max distance from patrol point to be counted as at point</summary>
        [SerializeField] private float _targetDistance = 0.5f;

        ///<summary>Patrol wait timer cooldown ID</summary>
        private static int _timePerPointID = AICooldownManager.GetHash("TimePerPoint");

        ///<summary>Get target patrol point</summary>
        ///<param name="context">Entity context</param>
        ///<param name="index">Patrol index</param>
        ///<returns>Target patrol destination</returns>
        public Vector3 GetPatrolTarget(StateMachineContext context, int index) {
            return _patrolPoints[index];
        }

        ///<summary>Used for first time initialisation</summary>
        public void Init() { }

        ///<summary>OnEnter call propagated from state</summary>
        ///<param name="context">Entity context</param>
        public void OnEnter(StateMachineContext context) { }

        ///<summary>OnUpdate call propagated from state</summary>
        ///<param name="context">Entity context</param>
        public void OnUpdate(StateMachineContext context, float dt) { }

        ///<summary>OnExit call propagated from state</summary>
        ///<param name="context">Entity context</param>
        public void OnExit(StateMachineContext context) { }

        ///<summary>Get next patrol index wrapped to patrol points length</summary>
        ///<param name="context">Entity context</param>
        ///<param name="index">Patrol index</param>
        ///<returns>Next patrol index</returns>
        public int Next(StateMachineContext context, int index) {
            return ++index % _patrolPoints.Length;
        }

        ///<summary>Get previous patrol index wrapped to patrol points length</summary>
        ///<param name="context">Entity context</param>
        ///<param name="index">Patrol index</param>
        ///<returns>Previous patrol index</returns>
        public int Prev(StateMachineContext context, int index) {
            if (index > 1) {
                return index - 1;
            } else {
                return _patrolPoints.Length - 1;
            }
        }

        ///<summary>Update wait timer when close enough to patrol point</summary>
        ///<param name="context">Entity context</param>
        ///<param name="dt">Time to tick timer by</param>
        public void TickPatrolPoint(StateMachineContext context, float dt) {
            if (!context.CooldownManager.Get(_timePerPointID).IsRunning) { context.CooldownManager.Get(_timePerPointID).Start(); }
            context.CooldownManager.Get(_timePerPointID).Update(dt);
        }

        ///<summary>Used to find if entity needs to advance to new patrol point</summary>
        ///<param name="context">Entity context</param>
        ///<param name="index">Patrol index</param>
        ///<returns>Has finished idling at patrol point</returns>
        public bool FinishedPatrolPoint(StateMachineContext context, int index) {
            return context.CooldownManager.Get(_timePerPointID).IsFinished;
        }

        ///<summary>Called upon finishing idling at patrol point</summary>
        ///<param name="context">Entity context</param>
        public void OnPatrolPointFinish(StateMachineContext context) {
            context.CooldownManager.Get(_timePerPointID).Reset();
        }

        ///<summary>Is close enough to patrol point to begin idling</summary>
        ///<param name="context">Entity context</param>
        ///<param name="index">Patrol index</param>
        ///<returns>True if close enough to patrol point</returns>
        public bool AtPatrolPoint(StateMachineContext context, Vector3 position, int index) {
            return Vector3.Distance(position, GetPatrolTarget(context, index)) <= _targetDistance;
        }
    }
}
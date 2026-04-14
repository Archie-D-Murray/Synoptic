using AI.HSM;

namespace AI {
    ///<summary>Extend this with your own types</summary>
    public enum AIState {
        None, Root, Idle, Wander, Patrol, Chase, Attack, Dead,
#if AI_EXAMPLES
        Ranged,
#endif
    }

    ///<summary>Root state is needed to act as base</summary>
    public class RootState : State {
        public RootState(StateMachine stateMachine, State parent) : base(stateMachine, parent) { }
    }
}
using AI.HSM;

namespace AI {
    public enum AIState { None, Root, Idle, Wander, Patrol, Chase, Attack, Dead }

    public class RootState : State {
        public RootState(StateMachine stateMachine, State parent) : base(stateMachine, parent) { }
    }
}
using AI.HSM;

namespace AI {
    public enum AIStates { None, Idle, Wander, Patrol, Chase, Attack }

    public class AIRootState : State {
        public AIRootState(StateMachine stateMachine, State parent) : base(stateMachine, parent) { }
    }
}
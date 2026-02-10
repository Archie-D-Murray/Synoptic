using AI.HSM;

namespace AI {
    public class DeadState : State {

        protected readonly StateMachineContext _context;

        public DeadState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }
    }
}
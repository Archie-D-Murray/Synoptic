using AI.HSM;

namespace AI {
    public class WanderState : State {

        protected readonly StateMachineContext _context;

        public WanderState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }
    }
}
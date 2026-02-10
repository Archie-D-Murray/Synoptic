using AI.HSM;

namespace AI {
    public class ChaseState : State {

        protected readonly StateMachineContext _context;

        public ChaseState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }
    }
}
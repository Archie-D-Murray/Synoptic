using AI.HSM;

namespace AI {
    public class PatrolState : State {

        protected readonly StateMachineContext _context;

        public PatrolState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }
    }
}
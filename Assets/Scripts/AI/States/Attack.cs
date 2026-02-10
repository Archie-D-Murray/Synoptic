using AI.HSM;

namespace AI {
    public class AttackState : State {

        protected readonly StateMachineContext _context;

        public AttackState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }
    }
}
namespace AI.HSM {
    public class StateBuilder {

        private StateMachine _stateMachine;
        private StateMachineContext _context;

        public void SetContext(StateMachineContext context) {
            _context = context;
        }

        public void SetStateMachine(StateMachine stateMachine) {
            _stateMachine = stateMachine;
        }

        public State Create(AIStates type, State parent = null) {
            return type switch {
                _ => new AIRootState(_stateMachine, parent),
            };
        }
    }
}
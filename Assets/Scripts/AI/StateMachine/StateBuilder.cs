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
                AIStates.Idle => new IdleState(_context, _stateMachine, parent),
                AIStates.Wander => new WanderState(_context, _stateMachine, parent),
                AIStates.Patrol => new PatrolState(_context, _stateMachine, parent),
                AIStates.Chase => new ChaseState(_context, _stateMachine, parent),
                AIStates.Attack => new AttackState(_context, _stateMachine, parent),
                AIStates.Dead => new DeadState(_context, _stateMachine, parent),
                _ => null
            };
        }
    }
}
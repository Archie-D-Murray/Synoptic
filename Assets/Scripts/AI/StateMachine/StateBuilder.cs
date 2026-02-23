using AI;

namespace AI.HSM {
    public class StateFactory {

        private StateMachine _stateMachine;
        private StateMachineContext _context;

        public StateFactory(StateMachine stateMachine, StateMachineContext context) {
            _stateMachine = stateMachine;
            _context = context;
        }

        public State Create(AIState type, State parent = null) {
            return type switch {
                AIState.None => new RootState(_stateMachine, null),
                AIState.Idle => new IdleState(_context, _stateMachine, parent),
                AIState.Wander => new WanderState(_context, _stateMachine, parent),
                AIState.Patrol => new PatrolState(_context, _stateMachine, parent),
                AIState.Chase => new ChaseState(_context, _stateMachine, parent),
                AIState.Attack => new AttackState(_context, _stateMachine, parent),
                AIState.Dead => new DeadState(_context, _stateMachine, parent),
                _ => null
            };
        }
    }
}
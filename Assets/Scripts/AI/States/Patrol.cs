using AI.HSM;

namespace AI {
    public class PatrolState : State {

        protected readonly StateMachineContext _context;
        protected int _patrolIndex = 0;

        public PatrolState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }

        protected override void OnEnter() {
            _context.PatrolInjector.OnEnter();
            _context.Movement.SetDestination(_context.PatrolInjector.GetPatrolTarget(_patrolIndex));
        }

        protected override void OnUpdate(float dt) {
            _context.PatrolInjector.OnUpdate(dt);
            if (_context.PatrolInjector.AtPatrolPoint(_context.Self.position, _patrolIndex)) {
                _context.PatrolInjector.TickPatrolPoint(dt);
            }

            if (_context.PatrolInjector.FinishedPatrolPoint(_patrolIndex)) {
                _patrolIndex = _context.PatrolInjector.Next(_patrolIndex);
                _context.Movement.SetDestination(_context.PatrolInjector.GetPatrolTarget(_patrolIndex));
            }
        }

        protected override void OnExit() {
            _context.PatrolInjector.OnExit();
            _context.Movement.SetDestination(_context.Self.position);
        }
    }
}
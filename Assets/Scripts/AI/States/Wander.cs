using AI.HSM;

namespace AI {
    public class WanderState : State {

        protected readonly StateMachineContext _context;

        public WanderState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }

        protected override void OnEnter() {
            _context.WanderInjector.OnEnter();
            _context.Movement.SetDestination(_context.WanderInjector.GetWanderPoint());
        }

        protected override void OnUpdate(float dt) {
            _context.WanderInjector.OnUpdate(dt);
            if (_context.WanderInjector.NextWanderPoint()) {
                _context.Movement.SetDestination(_context.WanderInjector.GetWanderPoint());
            }
        }

        protected override void OnExit() {
            _context.WanderInjector.OnExit();
            _context.Movement.SetDestination(_context.Self.position);
        }
    }
}
using AI.HSM;

namespace AI {
    public class IdleState : State {

        protected readonly StateMachineContext _context;

        public IdleState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }

        protected override void OnEnter() {
            _context.IdleInjector.OnEnter();
        }

        protected override void OnUpdate(float dt) {
            _context.IdleInjector.OnUpdate(dt);
        }

        protected override void OnExit() {
            _context.IdleInjector.OnExit();
        }
    }
}
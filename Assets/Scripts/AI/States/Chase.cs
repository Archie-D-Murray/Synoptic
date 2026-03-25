using AI.HSM;

using UnityEngine;

namespace AI {
    public class ChaseState : State {

        protected readonly StateMachineContext _context;
        protected Transform _target;

        public ChaseState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }

        protected override void OnEnter() {
            _context.ChaseInjector.OnEnter(_context);
        }

        protected override void OnUpdate(float dt) {
            _context.ChaseInjector.OnUpdate(_context, dt);

            _context.Movement.SetDestination(_context.Detector.TargetPosition);
            if (_context.Detector.JustLostTarget) {
                _context.ChaseInjector.StartLostTimer(_context);
            }
        }

        protected override void OnExit() {
            _context.ChaseInjector.OnExit(_context);
        }
    }
}
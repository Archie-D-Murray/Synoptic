using System.Collections.Generic;

using AI.Adapters;
using AI.HSM;

namespace AI {
    public class AttackState : State {

        protected readonly StateMachineContext _context;
        protected readonly List<AttackAdapter> _attacks = new List<AttackAdapter>(10);
        protected bool _isAttacking = false;
        protected float _attackTime = 0.0f;

        public AttackState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }

        protected override void OnEnter() {
            _context.AttackInjector.OnEnter(_context);
        }

        protected override void OnUpdate(float dt) {
            _context.AttackInjector.OnUpdate(_context, dt);
            if (_context.AttackInjector.CanAttack(_context)) {
                _context.AttackInjector.RestartAttackCooldown(_context);
                _attackTime = 0.0f;
                foreach (AttackAdapter attack in _context.AttackInjector.GetAttacks(_context)) {
                    int index = 0;
                    for (int i = 0; i < _attacks.Count; i++) {
                        if (_attacks[i].NormalizedTime >= attack.NormalizedTime) {
                            index = i;
                            break;
                        }
                    }
                    _attacks.Insert(index, attack);
                    if (attack.NormalizedTime == 0.0f) {
                        attack.OnEvent(null, null);
                    }
                }
            }

            _attackTime += dt;
            for (int i = 0; i < _attacks.Count;) {
                if (_attackTime / _context.AttackInjector.AttackTime(_context) >= _attacks[i].NormalizedTime) {
                    _attacks[i].OnEvent(null, null);
                    _attacks.RemoveAt(i);
                } else {
                    i++;
                }
            }
        }

        protected override void OnExit() {
            _context.AttackInjector.OnExit(_context);
        }
    }
}
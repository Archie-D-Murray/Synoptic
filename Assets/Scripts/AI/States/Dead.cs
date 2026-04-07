using AI.HSM;

namespace AI {

    ///<summary>A blank state to stop any transitions moving out of the current state</summary>
    public class DeadState : State {

        protected readonly StateMachineContext _context;

        public DeadState(StateMachineContext context, StateMachine stateMachine, State parent) : base(stateMachine, parent) {
            _context = context;
        }

        ///<summary>Plays death animation if present</summary>
        protected override void OnEnter() {
            _context.Animator.Play(Adapters.AIAnimationType.Dead);
        }
    }
}
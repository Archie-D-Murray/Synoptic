using UnityEngine;

namespace AI.HSM {
    public class StateFactory {

        private StateMachine _stateMachine;
        private StateMachineContext _context;

        ///<summary>Used for constructing states from state list in inspector</summary>
        ///<remarks>Only to be used with state machine with root already created</remarks>
        ///<param name="stateMachine">State machine to use in state initialisation</param>
        ///<param name="context">Context for state machine</param>
        public StateFactory(StateMachine stateMachine, StateMachineContext context) {
            _stateMachine = stateMachine;
            _context = context;
        }

        ///<summary>Creates state of type with parent</summary>
        ///<param name="type">State type to construct as real State</param>
        ///<param name="parent">Parent of state - can be null</param>
        ///<returns>Newly created state</returns>
        public State Create(AIState type, State parent = null) {
            return type switch {
                AIState.Idle => new IdleState(_context, _stateMachine, parent),
                AIState.Wander => new WanderState(_context, _stateMachine, parent),
                AIState.Patrol => new PatrolState(_context, _stateMachine, parent),
                AIState.Chase => new ChaseState(_context, _stateMachine, parent),
                AIState.Attack => new AttackState(_context, _stateMachine, parent),
                AIState.Dead => new DeadState(_context, _stateMachine, parent),
                AIState.Root => ReturnWithLog<RootState>(
                    new RootState(_stateMachine, null),
                    "Cannot create a root state as state machine must be intialised with root before factory creation"
                ),
                _ => null
            };
        }

        ///<summary>Used for invalid Create calls as switch returns cannot log and return</summary>
        private T ReturnWithLog<T>(T value, string message) {
            Debug.LogError($"[StateFactory (Object: {_context.name})] {message}", _context);
            return value;
        }
    }
}
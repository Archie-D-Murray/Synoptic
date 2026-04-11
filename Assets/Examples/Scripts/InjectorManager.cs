using System;

using AI.HSM;
using AI.Injectors;

using UnityEngine;

using Utilities;

namespace AI.Examples {
    [DefaultExecutionOrder(-99)]
    public class InjectorManager : Singleton<InjectorManager> {

        [SerializeField] public IIdleInjector Idle;
        [SerializeField] public IWanderInjector Wander;
        [SerializeField] public IPatrolInjector Patrol;
        [SerializeField] public IChaseInjector Chase;
        [SerializeField] public IAttackInjector Attack;

        private void OnValidate() {
            Idle = GetComponent<IIdleInjector>();
            Wander = GetComponent<IWanderInjector>();
            Patrol = GetComponent<IPatrolInjector>();
            Chase = GetComponent<IChaseInjector>();
            Attack = GetComponent<IAttackInjector>();
        }

        protected override void OnAwake() {
            Idle.Init();
            Wander.Init();
            Patrol.Init();
            Chase.Init();
            Attack.Init();
        }
    }

    [Serializable]
    public class DistributedStateDefinitions : IStateDefinition {
        public void InitInjectors(StateMachineContext ctx) {
            ctx.IdleInjector = InjectorManager.Instance.Idle;
            ctx.WanderInjector = InjectorManager.Instance.Wander;
            ctx.PatrolInjector = InjectorManager.Instance.Patrol;
            ctx.ChaseInjector = InjectorManager.Instance.Chase;
            ctx.AttackInjector = InjectorManager.Instance.Attack;
            ctx.IdleInjector.ContextInit(ctx);
            ctx.WanderInjector.ContextInit(ctx);
            ctx.PatrolInjector.ContextInit(ctx);
            ctx.ChaseInjector.ContextInit(ctx);
            ctx.AttackInjector.ContextInit(ctx);
        }

        public void InitTransitions(StateMachineContext ctx) {
            ctx.StateMachine.AddInitialState(ctx[AIState.Root], ctx[AIState.Idle]);
            // Idle
            // Disabled for distributed test
            // ctx.StateMachine.AddStateTransition(
            //     ctx[AIState.Idle],
            //     ctx[AIState.Wander],
            //     new AndPredicate(new LambdaPredicate(() => ctx.IdleInjector.DoneIdling(ctx)), new RandomChancePredicate(0.5f)));
            //
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Idle],
                ctx[AIState.Patrol],
                new LambdaPredicate(() => ctx.IdleInjector.DoneIdling(ctx)));

            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Idle],
                ctx[AIState.Chase],
                new LambdaPredicate(ctx.Detector.HasTarget));

            // Wander
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Wander],
                ctx[AIState.Chase],
                new LambdaPredicate(ctx.Detector.HasTarget));

            // Patrol
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Patrol],
                ctx[AIState.Chase],
                new LambdaPredicate(ctx.Detector.HasTarget));

            // Chase
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Chase],
                ctx[AIState.Attack],
                new LambdaPredicate(() => ctx.ChaseInjector.InAttackRange(ctx)));

            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Chase],
                ctx[AIState.Idle],
                new LambdaPredicate(() => ctx.ChaseInjector.LostTarget(ctx)));

            // Attack
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Attack],
                ctx[AIState.Chase],
                new LambdaPredicate(() => ctx.AttackInjector.UnableToAttack(ctx)));

        }

    }

    [Serializable]
    public class ManagerStateDefinitions : IStateDefinition {
        public void InitInjectors(StateMachineContext ctx) {
            ctx.IdleInjector = InjectorManager.Instance.Idle;
            ctx.WanderInjector = InjectorManager.Instance.Wander;
            ctx.PatrolInjector = InjectorManager.Instance.Patrol;
            ctx.ChaseInjector = InjectorManager.Instance.Chase;
            ctx.AttackInjector = InjectorManager.Instance.Attack;
            ctx.IdleInjector.ContextInit(ctx);
            ctx.WanderInjector.ContextInit(ctx);
            ctx.PatrolInjector.ContextInit(ctx);
            ctx.ChaseInjector.ContextInit(ctx);
            ctx.AttackInjector.ContextInit(ctx);
        }

        public void InitTransitions(StateMachineContext ctx) {
            ctx.StateMachine.AddInitialState(ctx[AIState.Root], ctx[AIState.Idle]);
            // Idle
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Idle],
                ctx[AIState.Wander],
                new AndPredicate(new LambdaPredicate(() => ctx.IdleInjector.DoneIdling(ctx)), new RandomChancePredicate(0.5f)));

            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Idle],
                ctx[AIState.Patrol],
                new AndPredicate(new LambdaPredicate(() => ctx.IdleInjector.DoneIdling(ctx)), new RandomChancePredicate(0.5f)));

            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Idle],
                ctx[AIState.Chase],
                new LambdaPredicate(ctx.Detector.HasTarget));

            // Wander
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Wander],
                ctx[AIState.Chase],
                new LambdaPredicate(ctx.Detector.HasTarget));

            // Patrol
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Patrol],
                ctx[AIState.Chase],
                new LambdaPredicate(ctx.Detector.HasTarget));

            // Chase
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Chase],
                ctx[AIState.Attack],
                new LambdaPredicate(() => ctx.ChaseInjector.InAttackRange(ctx)));

            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Chase],
                ctx[AIState.Idle],
                new LambdaPredicate(() => ctx.ChaseInjector.LostTarget(ctx)));

            // Attack
            ctx.StateMachine.AddStateTransition(
                ctx[AIState.Attack],
                ctx[AIState.Chase],
                new LambdaPredicate(() => ctx.AttackInjector.UnableToAttack(ctx)));

        }
    }
}
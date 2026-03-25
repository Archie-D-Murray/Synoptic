using System.Collections.Generic;

using AI.Adapters;
using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public interface IStateInjector {
        public void Init();

        ///
        ///<summary>Called once upon entering state</summary>
        ///
        public void OnEnter(StateMachineContext context);

        ///
        ///<summary>Called once per update cycle - it is recommended for this to be in Update()</summary>
        ///
        public void OnUpdate(StateMachineContext context, float dt);

        ///
        ///<summary>Called once upon state exit</summary>
        ///
        public void OnExit(StateMachineContext context);
    }

    public interface IIdleInjector : IStateInjector {
        public bool DoneIdling(StateMachineContext context);
    }

    public interface IWanderInjector : IStateInjector {
        public Vector3 GetWanderPoint(StateMachineContext context);
        public bool NextWanderPoint(StateMachineContext context);
    }

    public interface IPatrolInjector : IStateInjector {
        public Vector3 GetPatrolTarget(StateMachineContext context, int index);
        public int Next(StateMachineContext context, int index);
        public int Prev(StateMachineContext context, int index);

        public void TickPatrolPoint(StateMachineContext context, float dt);
        public bool FinishedPatrolPoint(StateMachineContext context, int index);
        public bool AtPatrolPoint(StateMachineContext context, Vector3 position, int index);
    }

    public interface IChaseInjector : IStateInjector {
        public bool LostTarget(StateMachineContext context);
        public void StartLostTimer(StateMachineContext context);
        public bool SwitchToAttack(StateMachineContext context);
    }

    public interface IAttackInjector : IStateInjector {
        public bool CanAttack(StateMachineContext context);
        public bool SwitchToChase(StateMachineContext context);
        public float AttackTime(StateMachineContext context);
        public List<AttackAdapter> GetAttacks(StateMachineContext context);
        public void RestartAttackCooldown(StateMachineContext context);
    }
}
using System.Collections.Generic;

using AI.Adapters;
using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public interface IStateInjector {

        ///
        ///<summary>Called once upon state machine initialisation</summary>
        ///
        public void ProvideState(StateMachineContext context);

        ///
        ///<summary>Called once upon entering state</summary>
        ///
        public void OnEnter();

        ///
        ///<summary>Called once per update cycle - it is recommended for this to be in Update()</summary>
        ///
        public void OnUpdate(float dt);

        ///
        ///<summary>Called once upon state exit</summary>
        ///
        public void OnExit();
    }

    public interface IIdleInjector : IStateInjector {
        public bool DoneIdling();
    }

    public interface IWanderInjector : IStateInjector {
        public Vector3 GetWanderPoint();
        public bool NextWanderPoint();
    }

    public interface IPatrolInjector : IStateInjector {
        public Vector3 GetPatrolTarget(int index);
        public int Next(int index);
        public int Prev(int index);

        public void TickPatrolPoint(float dt);
        public bool FinishedPatrolPoint(int index);
        public bool AtPatrolPoint(Vector3 position, int index);
    }

    public interface IChaseInjector : IStateInjector {
        public bool LostTarget();
        public void StartLostTimer();
        public bool SwitchToAttack();
    }

    public interface IAttackInjector : IStateInjector {
        public bool CanAttack();
        public bool SwitchToChase();
        public float AttackTime();
        public List<AttackAdapter> GetAttacks();
        public void RestartAttackCooldown();
    }
}
using AI.Adapters;
using AI.HSM;

using UnityEngine;

namespace AI.Injectors {
    public interface IStateInjector {
        public void ProvideState(StateMachineContext context);
        public void OnEnter();
        public void OnUpdate(float dt);
        public void OnExit();
    }

    public interface IIdleInjector : IStateInjector {
        public bool DoneIdling();
    }

    public interface IWanderInjector : IStateInjector {
        public Vector3 GetWanderPoint();
    }

    public interface IPatrolInjector : IStateInjector {
        public Vector3 GetPatrolTarget();
        public void Next();
        public void Prev();
    }

    public interface IChaseInjector : IStateInjector {
        public bool LostTarget();
        public bool SwitchToLastAction();
        public bool SwitchToAttack();
    }

    public interface IAttackInjector : IStateInjector {
        public float GetAttackCooldown();
        public void AddAttack(AttackData data);
    }
}
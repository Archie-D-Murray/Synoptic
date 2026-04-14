#if AI_EXAMPLES

using AI.Injectors;

namespace AI.HSM {
    public partial class StateMachineContext {
        public float AttackRange = 5.0f;
        public float RangedRange = 7.5f;
        public float RangedMovementLockout = 0.5f;

        public IAttackInjector RangedInjector;
    }
}

#endif
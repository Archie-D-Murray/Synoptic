using UnityEngine;

namespace AI.Adapters {

    public class AttackData {
        public Transform Entity;
        public Transform[] Other;
    }

    public abstract class AttackAdapter {
        protected float _normalizedTime;

        public abstract void OnEvent(AnimationClip clip, AttackData data);
    }

    public class NullAttackAdapter : AttackAdapter {

        public override void OnEvent(AnimationClip clip, AttackData data) { }
    }
}
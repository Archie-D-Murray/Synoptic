using UnityEngine;

namespace AI.Adapters {

    public class AttackData {
        public Transform Entity;
        public Transform[] Other;
    }

    public abstract class AttackAdapter {
        protected float _normalizedTime;

        public float NormalizedTime => _normalizedTime;

        public abstract void OnEvent(AnimationClip clip, AttackData data);
    }

    public class NullAttackAdapter : AttackAdapter {

        public override void OnEvent(AnimationClip clip, AttackData data) {
            Debug.Log("[Attack Adapter]: Null attack called");
        }
    }
}
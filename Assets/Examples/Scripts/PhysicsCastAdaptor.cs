using UnityEngine;

using AI.Adapters;
using AI.HSM;

namespace AI.Examples {

    [System.Serializable]
    public class PhysicsCastAdaptor : AttackAdaptor {
        [SerializeField] private float _range;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private float _damage = 5.0f;

        public PhysicsCastAdaptor() {
            _normalizedTime = 0.5f;
            _range = 5.0f;
            _layer = -1;
        }

        public PhysicsCastAdaptor(float normalizedTime, float range, LayerMask layer) {
            _normalizedTime = normalizedTime;
            _range = range;
            _layer = layer;
        }

        public override void OnEvent(AnimationClip clip, StateMachineContext context) {
            foreach (Collider hit in Physics.OverlapSphere(context.Position, _range, _layer)) {
                if (hit.transform != context.transform && hit.TryGetComponent(out IDamageable damageable)) {
                    damageable.Damage(new DamageSource(_damage, context.gameObject, hit.gameObject));
                }
            }
        }
    }

}
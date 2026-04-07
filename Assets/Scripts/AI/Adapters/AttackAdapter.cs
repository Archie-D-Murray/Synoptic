using AI.HSM;

using UnityEngine;

namespace AI.Adapters {

    ///<summary>Base for all attacks to derive from</summary>
    ///<summary></summary>
    ///<summary>
    ///<example>A physics cast based example may look like:
    ///<code>
    ///using AI.HSM;
    ///using UnityEngine;
    ///
    ///public class PhysicsCastAdaptor : AttackAdapter {
    ///    [SerializeField] private float _range;
    ///    [SerializeField] private LayerMask _layer;
    ///
    ///    public PhysicsCastAdapator() {
    ///        _normalizedTime = 0.5f;
    ///        _range = 5.0f;
    ///        _layer = Mathf.Pow(2, LayerMask.NameToLayer("Player"));
    ///    }
    ///
    ///    public PhysicsCastAdapator(float normalizedTime, float range, LayerMask layer) {
    ///        _normalizedTime = normalizedTime;
    ///        _range = range;
    ///        _layer = layer;
    ///    }
    ///
    ///    public override void OnEvent(AnimationClip clip, StateMachineContext context) {
    ///        foreach (Collider hit in Physics.OverlapSphere(context.Position, _range, _layer)) {
    ///            if (hit.TryGetComponent(out IDamageable damageable)) {
    ///                damageable.Damage(_damage);
    ///            }
    ///        }
    ///    }
    ///}
    ///</code>
    ///</example>
    ///</summary>
    [System.Serializable]
    public abstract class AttackAdapter {
        protected float _normalizedTime;

        ///<summary>Normalized time for attack anim to call attack once</summary>
        public float NormalizedTime => _normalizedTime;

        ///<summary>Called when normalized attack duration reaches <c>NormalizedTime</c></summary>
        ///<remarks>Could be used to do something like a phyics cast or process ColliderCollector results</remarks>
        ///<param name="clip">Most weighted clip currently playing - used if actual attack time is needed</param>
        ///<param name="context">Entity that attack was called from</param>
        public abstract void OnEvent(AnimationClip clip, StateMachineContext context);
    }

    ///<summary>Attack that just logs an attack has happened at the start of an attack</summary>
    [System.Serializable]
    public class NullAttackAdapter : AttackAdapter {

        public override void OnEvent(AnimationClip clip, StateMachineContext context) {
            Debug.Log($"[Attack Adapter]: Null attack called on entity - {context.name}");
        }
    }
}
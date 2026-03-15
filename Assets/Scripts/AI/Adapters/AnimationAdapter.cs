using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace AI.Adapters {

    public enum AIAnimationType { None, Idle, Wander, Patrol, Chase, Attack }

    [Serializable]
    public class Animation {
        public AIAnimationType AnimationType;
        public string StateName;
        public string Layer;
    }

    public class AnimationAdapter : MonoBehaviour {
        [SerializeField] protected Animation[] _animations;
        [SerializeField] protected AIAnimationType _animation;
        [SerializeField] protected AIAnimationType _default;
        [SerializeField] protected bool _debugMessages = false;

        protected Dictionary<AIAnimationType, int> _lookup;
        protected Animator _animator;
        protected int _currentHash;

        public Animator Animator => _animator;

        protected virtual void Awake() {
            _animator = GetComponentInChildren<Animator>();
            _lookup = new Dictionary<AIAnimationType, int>(_animations.Length);
            for (int i = 0; i < _animations.Length; i++) {
                _lookup.Add(_animations[i].AnimationType, i);
            }

            if (_default != AIAnimationType.None) {
                Play(_default, 0.0f);
            }
        }

        public virtual void Play(AIAnimationType animation, float normalizedTransitionDuration = 0.1f) {
            if (_lookup.TryGetValue(animation, out int hash)) {
                if (_currentHash == hash) { return; }
                _animator.CrossFade(hash, normalizedTransitionDuration);
                _currentHash = hash;
                _animation = animation;
            } else if (_debugMessages) {
                Debug.LogWarning($"[Animation Adapter]: {name} Animator did not have entry {animation}...", this);
            }
        }

        public float GetCurrentAnimationLength(int layer = -1) {
            return _animator.GetCurrentAnimatorClipInfo(layer).Average(clip => clip.clip.length * clip.weight);
        }
    }
}
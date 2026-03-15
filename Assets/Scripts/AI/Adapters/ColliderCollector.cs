using System.Collections.Generic;

using UnityEngine;

namespace AI.Adapters {
    public class ColliderCollector : MonoBehaviour {
        public readonly HashSet<Transform> Transforms = new HashSet<Transform>();
        public readonly HashSet<Collider> Colliders = new HashSet<Collider>();

        [SerializeField] private bool _allowCollection = false;
        [SerializeField] private int _defaultCapacity = 32;

        private void Awake() {
            Transforms.EnsureCapacity(_defaultCapacity);
            Colliders.EnsureCapacity(_defaultCapacity);
        }

        public void EnableCollection() {
            _allowCollection = true;
        }

        public void DisableCollection(bool clearCollections = false) {
            _allowCollection = false;

            if (clearCollections) {
                ClearCollections();
            }
        }

        public bool ToggleCollection(bool enabled) {
            _allowCollection = enabled;
            return _allowCollection;
        }

        public void ClearCollections() {
            Transforms.Clear();
            Colliders.Clear();
        }

        private void OnTriggerEnter(Collider collider) {
            if (_allowCollection && Transforms.Contains(collider.transform)) {
                Transforms.Add(collider.transform);
                Colliders.Add(collider);
            }
        }
    }
}
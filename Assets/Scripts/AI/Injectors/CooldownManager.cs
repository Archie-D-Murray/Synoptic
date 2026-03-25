using UnityEngine;
using Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace AI {

    [Serializable]
    public class AICooldown {
        public string Name;
        public int ID;
        public CountDownTimer Timer;
        [HideInInspector] public bool Managed = true;

        public AICooldown(string name, float cooldown, bool managed = true) {
            Name = name;
            ID = AICooldownManager.GetHash(name);
            Timer = new CountDownTimer(cooldown);
            Managed = managed;
        }

        public AICooldown(string name, int id, float cooldown, bool managed = true) {
            Name = name;
            ID = id;
            Timer = new CountDownTimer(cooldown);
            Managed = managed;
        }
    }

    [Serializable]
    public class AICooldownManager {
        [SerializeField] private List<AICooldown> _cooldowns;
        private readonly Dictionary<int, int> _cooldownIDs;

        public AICooldownManager() {
            _cooldowns = new List<AICooldown>();
            _cooldownIDs = new Dictionary<int, int>();
        }
        public AICooldownManager(AICooldown[] cooldowns) {
            _cooldowns = cooldowns.ToList();
            _cooldownIDs = new Dictionary<int, int>();
            for (int i = 0; i < _cooldowns.Count; i++) {
                _cooldownIDs.Add(_cooldowns[i].ID, i);
            }
        }

        public static int GetHash(string name) {
            return Animator.StringToHash(name);
        }

        public void Update(float dt) {
            foreach (AICooldown cooldown in _cooldowns) {
                if (cooldown.Managed) {
                    cooldown.Timer.Update(dt);
                }
            }
        }

        public int CreateCooldown(float time, string name, bool managed = false) {
            if (name == string.Empty) { name = $"[{_cooldowns.Count}] Cooldown"; }
            int id = GetHash(name);
            _cooldowns.Add(new AICooldown(name, id, time, managed));
            _cooldownIDs.Add(id, _cooldowns.Count - 1);

            return id;
        }

        public CountDownTimer Get(int id) {
            if (_cooldownIDs.TryGetValue(id, out int index)) {
                return _cooldowns[index].Timer;
            } else {
                return null;
            }
        }

        public bool IsFinished(int id) {
            return Get(id)?.IsFinished ?? false;
        }
    }
}
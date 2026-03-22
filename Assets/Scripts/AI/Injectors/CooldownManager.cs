using UnityEngine;
using Utilities;

using System;
using System.Collections.Generic;

namespace AI {

    [Serializable]
    public class AICooldown {
        public string Name;
        public int ID;
        public CountDownTimer Timer;
        [HideInInspector] public bool Managed = true;

        public AICooldown(string name, int id, float cooldown, bool managed = true) {
            Name = name;
            ID = id;
            Timer = new CountDownTimer(cooldown);
            Managed = managed;
        }
    }

    public class AICooldownManager {
        [SerializeField] private List<AICooldown> _cooldowns = new List<AICooldown>();
        int _nextCooldown = 0;
        private readonly Dictionary<int, int> _cooldownIDs = new Dictionary<int, int>();

        public void Update(float dt) {
            foreach (AICooldown cooldown in _cooldowns) {
                if (cooldown.Managed) {
                    cooldown.Timer.Update(dt);
                }
            }
        }

        public int CreateCooldown(float time, string name, bool managed = true) {
            int id = ++_nextCooldown;
            if (name == string.Empty) { name = $"[{id}] Cooldown"; } else { name = $"[{id}] {name}"; }
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
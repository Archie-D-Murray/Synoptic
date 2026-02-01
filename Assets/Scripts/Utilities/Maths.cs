using UnityEngine;

namespace Utilities {
    public static class Maths {
        public static float SineSmooth(float delta) {
            delta = Mathf.Clamp01(delta);
            return Mathf.Lerp(0.0f, 1.0f, 0.5f + 0.5f * (Mathf.Sin(Mathf.PI * delta - 0.5f * Mathf.PI)));
        }

        public static float EaseOutBack(float value) {
            return 1 + (1.70158f + 1) * Mathf.Pow(value - 1, 3) + 1.70158f * Mathf.Pow(value - 1, 2);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Utilities;

using Random = UnityEngine.Random;

public static class Extensions {
    public static IEnumerable<Enum> GetFlags(this Enum flags) {
        return Enum.GetValues(flags.GetType()).Cast<Enum>().Where(flags.HasFlag);
    }


    /// <summary>
    /// Gets, or adds if doesn't contain a component
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="gameObject">GameObject to get component from</param>
    /// <returns>Component</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
        T component = gameObject.GetComponent<T>();
        if (!component) {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// Returns true if a GameObject has a component of type T
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="gameObject">GameObject to check for component on</param>
    /// <returns>If the component is present</returns>
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component {
        return gameObject.GetComponent<T>() != null;
    }

    public static T OrNull<T>(this T obj) where T : UnityEngine.Object {
        return obj ? obj : null;
    }

    public static void FlashColour(this SpriteRenderer spriteRenderer, Color colour, float time, MonoBehaviour monoBehaviour) {
        monoBehaviour.StartCoroutine(Flash(spriteRenderer, colour, time));
    }

    private static IEnumerator Flash(SpriteRenderer spriteRenderer, Color colour, float time) {
        Color original = spriteRenderer.material.color;
        spriteRenderer.material.color = colour;
        yield return new WaitForSeconds(time);
        spriteRenderer.material.color = original;
    }

    public static void FlashDamage(this SpriteRenderer spriteRenderer, Material flashMaterial, Material originalMaterial, float duration, MonoBehaviour monoBehaviour) {
        monoBehaviour.StartCoroutine(FlashDamage(spriteRenderer, flashMaterial, originalMaterial, duration));
    }

    private static IEnumerator FlashDamage(SpriteRenderer spriteRenderer, Material flashMaterial, Material originalMaterial, float duration) {
        spriteRenderer.material = flashMaterial;
        yield return Yielders.WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
    }

    public static void Fade(this SpriteRenderer spriteRenderer, Color original, Color target, float duration, MonoBehaviour monoBehaviour) {
        monoBehaviour.StartCoroutine(Fade(spriteRenderer, original, target, duration));
    }

    private static IEnumerator Fade(SpriteRenderer spriteRenderer, Color original, Color target, float duration) {
        float timer = 0.0f;
        while (timer < duration) {
            timer += Time.fixedDeltaTime;
            spriteRenderer.color = Color.Lerp(original, target, timer / duration);
            yield return Yielders.WaitForFixedUpdate;
        }
        spriteRenderer.color = target;
    }

    public static Coroutine FadeAlpha(this SpriteRenderer spriteRenderer, float speed, bool fadeToTransparent, MonoBehaviour monoBehaviour) {
        return monoBehaviour.StartCoroutine(FadeAlpha(spriteRenderer, speed, fadeToTransparent));
    }

    private static IEnumerator FadeAlpha(SpriteRenderer spriteRenderer, float speed, bool fadeToTransparent) {
        Color rgb = spriteRenderer.color;
        float target = fadeToTransparent ? 0.0f : 1.0f;
        while (spriteRenderer.color.a != target) {
            rgb.a = Mathf.MoveTowards(rgb.a, target, speed * Time.fixedDeltaTime);
            spriteRenderer.color = rgb;
            yield return Yielders.WaitForFixedUpdate;
        }
    }

    public static void FadeCanvas(this CanvasGroup canvasGroup, float fadeSpeed, bool fadeToTransparent, MonoBehaviour monoBehaviour, bool debug = false) {
        monoBehaviour.StartCoroutine(CanvasFade(canvasGroup, fadeSpeed, fadeToTransparent, debug));
    }

    public static Coroutine FadeCanvasC(this CanvasGroup canvasGroup, float fadeSpeed, bool fadeToTransparent, MonoBehaviour monoBehaviour, bool debug = false) {
        return monoBehaviour.StartCoroutine(CanvasFade(canvasGroup, fadeSpeed, fadeToTransparent, debug));
    }

    private static IEnumerator CanvasFade(CanvasGroup canvasGroup, float fadeSpeed, bool fadeToTransparent, bool debug) {
        float target = fadeToTransparent ? 0.0f : 1.0f;
        float deltaTime = 0;
        float lastTime = Time.time;
        while (canvasGroup.alpha != target) {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, fadeSpeed * Time.fixedDeltaTime);
            if (debug) {
                deltaTime = Time.time - lastTime;
                lastTime = Time.time;
                Debug.Log($"Faded to {canvasGroup.alpha} at step: {fadeSpeed * Time.fixedDeltaTime} and deltaTime {deltaTime}");
            }
            yield return Yielders.WaitForFixedUpdate;
        }
        if (fadeToTransparent) {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        } else {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public static Vector2 Clamp(this Vector2 position, Vector2 min, Vector2 max) {
        return new Vector2(
            Mathf.Clamp(position.x, min.x, max.x),
            Mathf.Clamp(position.y, min.y, max.y)
        );
    }

    public static bool IsFinite(this Vector3 position) {
        return float.IsFinite(position.x) && float.IsFinite(position.y) && float.IsFinite(position.z);
    }

    public static void SetPositionAndRotation(this Transform transform, Transform other) {
        if (other) {
            transform.SetPositionAndRotation(other.position, other.rotation);
        }
    }

    public static Vector3 ToXZ(this Vector2 vector) {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector3 WithY(this Vector3 vector, float y) {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 WithZ(this Vector3 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector3 Offset(this Vector3 vector, float x = 0.0f, float y = 0.0f, float z = 0.0f) {
        return vector + new Vector3(x, y, z);
    }

    public static T GetRandomValue<T>(this T[] array) {
        return array[Random.Range(0, array.Length)];
    }

    public static bool InBounds<T>(this List<T> collection, int index) {
        return index >= 0 && index < collection.Count;
    }

    public static float AngleTo(this Vector2 origin, Vector2 point) {
        return Vector2.SignedAngle(Vector2.up, (point - origin).normalized);
    }

    public static bool InRange(this float value, float min, float max) {
        return value <= max && value >= min;
    }

    public static Quaternion RotationTo(this Vector2 origin, Vector2 point) {
        return Quaternion.AngleAxis(origin.AngleTo(point), Vector3.forward);
    }

    public static Quaternion RotationCardinalTo(this Vector2 origin, Vector2 point) {
        float angle = origin.AngleTo(point);
        angle = (int)(Mathf.RoundToInt(angle < 0 ? 360 + angle : angle + 45) / 90) * 90;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static Collider2D Closest(this Collider2D[] colliders, Vector2 position) {
        Collider2D returnValue = null;
        float closest = float.MaxValue;
        foreach (Collider2D collider in colliders) {
            float distance = Vector2.Distance(position, collider.transform.position);
            if (distance < closest) {
                closest = distance;
                returnValue = collider;
            }
        }

        return returnValue;
    }

    public static bool Populated<T>(this T[] array) {
        return array != null && array.Length > 0;
    }

    public static Color WithAlpha(this Color colour, float alpha = 0.0f) {
        return new Color(colour.r, colour.g, colour.b, alpha);
    }

    public static AnimationClip GetRuntimeClip(this Animator animator, int hash) {
        return animator.runtimeAnimatorController.OrNull()?.animationClips.FirstOrDefault(clip => Animator.StringToHash(clip.name) == hash) ?? null;
    }
}
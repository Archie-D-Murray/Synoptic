using Utilities;

using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class FadeScreen : Singleton<FadeScreen> {

        [SerializeField] private float _fadeSpeed = 2.0f;
        [SerializeField] private Image _fade;

        public float MaxFadeTime => 1.0f / _fadeSpeed;

        private Coroutine _fadeCoroutine = null;

        protected override void OnAutoCreate() {
            transform.parent = FindObjectsByType<Canvas>(FindObjectsSortMode.None).First(canvas => canvas.renderMode != RenderMode.WorldSpace).transform;
            gameObject.GetOrAddComponent<Image>().color = Color.black.WithAlpha(0.0f);
        }

        private void Start() {
            _fade = GetComponent<Image>();
            _fade.color = Color.black;
            Clear();
        }

        public void Black() {
            if (_fadeCoroutine != null) {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(Fade(_fadeSpeed, false));
        }

        public void Clear() {
            if (_fadeCoroutine != null) {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(Fade(_fadeSpeed, true));
        }

        private IEnumerator Fade(float _fadeSpeed, bool toTransparent, bool setInitialAlpha = false) {
            if (setInitialAlpha) {
                _fade.color = _fade.color.WithAlpha(toTransparent ? 1.0f : 0.0f);
            }
            float target = toTransparent ? 0.0f : 1.0f;
            while (_fade.color.a != target) {
                _fade.color = _fade.color.WithAlpha(Mathf.MoveTowards(_fade.color.a, target, _fadeSpeed * Time.fixedDeltaTime));
                yield return Yielders.WaitForFixedUpdate;
            }
        }
    }
}
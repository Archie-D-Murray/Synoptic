using System;

using UnityEngine;

namespace Utilities {
    public class Helpers : Singleton<Helpers> {

        public readonly Vector2 Offset = new Vector2(0.5f, 0.5f);

        protected override void Awake() {
            base.Awake();
            MainCamera = Camera.main;
            PhysicsFPS = Mathf.Round(1.0f / Time.fixedUnscaledDeltaTime);
            Debug.Log($"Target physics frameRate => {PhysicsFPS}");
        }

        public Camera MainCamera;
        private Vector2 _mousePosition;
        public Vector2 TileMapMousePosition => Vector2Int.FloorToInt(_mousePosition) + Offset;
        public static Vector2 FromRadians(float radians) {
            return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        }

        public float AngleToMouse(Transform obj) {
            return Vector2.SignedAngle(
                Vector2.up,
                ((Vector2)MainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)obj.position).normalized
            );
        }

        public float AngleToMouseOpposite(Transform obj) {
            return Vector2.SignedAngle(
                ((Vector2)MainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)obj.position).normalized,
                Vector2.up
            );
        }

        public Vector2 GetWorldMousePosition() {
            return MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        public Vector2 VectorToMouse(Vector2 position) {
            return (GetWorldMousePosition() - position).normalized;
        }

        private void FixedUpdate() {
            _mousePosition = GetWorldMousePosition();
        }

        public static Quaternion Look2D(Vector2 from, Vector2 to) {
            return Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, (to - from).normalized), Vector3.forward);
        }

        public static float PhysicsFPS = 50.0f;

        public static float NormalizedFixedDeltaTime => Time.fixedDeltaTime * PhysicsFPS;

        public static float ClampAngle(float degrees, float min, float max) {
            while (degrees < -360.0f) { degrees += 360.0f; }
            while (degrees > +360.0f) { degrees -= 360.0f; }
            return Mathf.Clamp(degrees, min, max);
        }
    }
}
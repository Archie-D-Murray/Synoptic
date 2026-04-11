#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.Linq;
using AI.Adapters;

[CustomPropertyDrawer(typeof(AttackAdaptor), true)]
public class AttackAdaptorEditor : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (property.managedReferenceValue == null) {
            return EditorGUIUtility.singleLineHeight;
        } else if (property.managedReferenceValue != null && _props == null) {
            _props = GetProps(property);
        }
        return EditorGUIUtility.singleLineHeight * (_props.Length) + 2 * (_props.Length + 1);
    }

    private SerializedProperty[] _props;

    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        using (EditorGUI.ChangeCheckScope changed = new EditorGUI.ChangeCheckScope()) {
            if (changed.changed || (_props == null && property.managedReferenceValue != null)) {
                _props = GetProps(property);
            }
        } // Change Check Scope is IDisposable

        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (property.managedReferenceValue != null) {
            foreach (SerializedProperty prop in _props) {
                if (prop != null) {
                    EditorGUI.PropertyField(rect, prop);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                }
            }
        } else {
            EditorGUI.LabelField(rect, "Null");
        }

        EditorGUI.EndProperty();
    }

    ///<summary>This is fairly nasty - try to cache return value where possible</summary>
    ///<param name="reference">Non null ref to object (will error out if null)</param>
    ///<returns>Array of properties in reverse order as for some reason they are backwards...</returns>
    private SerializedProperty[] GetProps(SerializedProperty reference) {
        return reference.managedReferenceValue.GetType()
            .GetFields(flags)
            .Select(field => reference.FindPropertyRelative(field.Name))
            .Reverse()
            .ToArray();
    }
}

#endif
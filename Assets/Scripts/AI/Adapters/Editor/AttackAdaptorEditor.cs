#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.Linq;
using AI.Adapters;

[CustomPropertyDrawer(typeof(AttackAdaptor), true)]
public class AttackAdaptorEditor : PropertyDrawer {

    float fieldCount = 4;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight * (fieldCount) + 2 * (fieldCount + 1);
    }

    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (property.managedReferenceValue != null) {
            fieldCount = 0;
            foreach (FieldInfo field in property.managedReferenceValue.GetType().GetFields(flags).Reverse()) {
                SerializedProperty prop = property.FindPropertyRelative(field.Name);
                if (prop != null) {
                    EditorGUI.PropertyField(rect, prop);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    fieldCount++;
                } else {
                    EditorGUI.LabelField(rect, $"Field: {field.Name} could not be gotten");
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                }
            }
        } else {
            EditorGUI.LabelField(rect, "Null");
        }

        EditorGUI.EndProperty();
    }
}

#endif
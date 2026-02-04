using UnityEditor;

using UnityEngine;

using Utilities;

[CustomPropertyDrawer(typeof(SerializedPairBase), true)]
public class SerializedPairDrawer : PropertyDrawer {

    const float Padding = 5.0f;
    const float KeyMinWidth = 80.0f;
    const float ValueMinWidth = 120.0f;
    const float ValueWeightPadding = 0.05f;

    public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label) {
        SerializedProperty key = property.FindPropertyRelative("Key");
        SerializedProperty val = property.FindPropertyRelative("Value");

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        float width = position.width - Padding;

        float keyWeight = 0.4f;
        if (key.propertyType == SerializedPropertyType.ObjectReference) {
            keyWeight += 0.1f;
        } else if (key.propertyType == SerializedPropertyType.Enum) {
            keyWeight -= 0.1f;
        }
        float valueWeight = 1.0f - keyWeight - ValueWeightPadding;

        float keyWidth = Mathf.Max(KeyMinWidth, width * keyWeight);
        float valueWidth = Mathf.Max(ValueMinWidth, width * valueWeight);

        Rect keyRect = new Rect(position.x, position.y, keyWidth, position.height);
        Rect valueRect = new Rect(
            keyRect.xMax + Padding + width * ValueWeightPadding,
            position.y,
            valueWidth,
            position.height
        );

        EditorGUI.PropertyField(keyRect, key, GUIContent.none, true);
        EditorGUI.PropertyField(valueRect, val, GUIContent.none, true);

        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return Mathf.Max(
            EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Key"), true),
            EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Value"), true)
        );
    }
}
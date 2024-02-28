using UnityEngine;
using UnityEditor;
using BehaviorTree;

namespace BehaviorTreeEditor
{
    [CustomPropertyDrawer(typeof(CompareFloat))]
    public class CompareDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty leftValueProp = property.FindPropertyRelative("_leftValue");
            SerializedProperty operatorProp = property.FindPropertyRelative("_operator");
            SerializedProperty rightValueProp = property.FindPropertyRelative("_rightValue");

            float fieldWidth = (position.width - 6f) / 3f;

            Rect leftRect = new Rect(position.x, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            Rect operatorRect = new Rect(position.x - fieldWidth + 6f ,position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            Rect rightRect = new Rect(position.x + 2f * (fieldWidth + 2f), position.y, fieldWidth, EditorGUIUtility.singleLineHeight);

            // 描画
            EditorGUI.PropertyField(leftRect, leftValueProp, GUIContent.none);
            EditorGUI.PropertyField(operatorRect, operatorProp, GUIContent.none);
            EditorGUI.PropertyField(rightRect, rightValueProp, GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}

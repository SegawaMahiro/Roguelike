using BehaviorTree;
using UnityEditor;
using UnityEngine;

namespace BehaviorTreeEditor
{
    [CustomPropertyDrawer(typeof(BaseVariableViewer), true)]
    public class VariableReferenceDrawer : PropertyDrawer
    {
        private const float ToggleWidthPercentage = 0.3f;
        private const float ValueWidthPercentage = 0.7f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty isKeyInput = property.FindPropertyRelative("_isKeyInput");
            SerializedProperty key = property.FindPropertyRelative("_key");
            SerializedProperty constantValue = property.FindPropertyRelative("_rawValue");
            SerializedProperty blackboard = property.FindPropertyRelative("_blackboard");

            var rootComponent = property.serializedObject.targetObject as MonoBehaviour;
            blackboard.objectReferenceValue = GetRootBlackboard(rootComponent);
            property.serializedObject.ApplyModifiedProperties();


            float toggleWidth = position.width * ToggleWidthPercentage;
            float valueWidth = position.width * ValueWidthPercentage;

            Rect toggleRect = new Rect(position.x, position.y, toggleWidth, position.height);
            Rect valueRect = new Rect(position.x + toggleWidth, position.y, valueWidth, position.height);

            EditorGUI.PropertyField(toggleRect, isKeyInput, GUIContent.none);

            if (isKeyInput.boolValue) {
                EditorGUI.PropertyField(valueRect, key, GUIContent.none);
            }
            else {
                EditorGUI.PropertyField(valueRect, constantValue, GUIContent.none);
            }

            string valueTypeLabel = "";
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), valueTypeLabel);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight ;
        }

        protected BehaviorTreeBlackBoard GetRootBlackboard(MonoBehaviour mono) {
            var root = mono.gameObject;
            return root.GetComponent<BehaviorTreeBlackBoard>();
        }
    }
}

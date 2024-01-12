using UnityEngine;
using UnityEditor;
using Assets.Scripts.Utils;
using System;

[CustomPropertyDrawer(typeof(PropertyField2D<>))]
public class PropertyField2DDrawer : PropertyDrawer
{
    private int _labelSpaceWidth = 20; // 横幅を変更
    private int _labelSpaceHeight = 20; // 高さを変更

    private const float PROPERTY_SPACE = 1.3f; // property間のスペース倍率

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.PrefixLabel(position, label);

        // 縦列に入っているすべての横列
        SerializedProperty column = property.FindPropertyRelative("_column");
        // 縦の長さ
        SerializedProperty columnLength = property.FindPropertyRelative("_columnLength");
        // 横の長さ
        SerializedProperty rowLength = property.FindPropertyRelative("_rowLength");

        SerializedProperty labelWidth = property.FindPropertyRelative("_labelWidth");
        SerializedProperty labelHeight = property.FindPropertyRelative("_labelHeight");

        _labelSpaceHeight = labelHeight.intValue;
        _labelSpaceWidth = labelWidth.intValue;

        Rect inspectorRect = position;
        inspectorRect.y += _labelSpaceHeight;
        

        for (int i = 0; i < columnLength.intValue; i++) {
            // その縦列に入っている横の要素
            SerializedProperty row = column.GetArrayElementAtIndex(i).FindPropertyRelative("Row");

            inspectorRect.height = _labelSpaceHeight;
            inspectorRect.width = _labelSpaceWidth;

            for (int j = 0; j < rowLength.intValue; j++) {
                EditorGUI.PropertyField(inspectorRect, row.GetArrayElementAtIndex(j), GUIContent.none);
                inspectorRect.x += inspectorRect.width;
            }

            inspectorRect.x = position.x;
            inspectorRect.y += _labelSpaceHeight;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty columnLength = property.FindPropertyRelative("_columnLength");
        return columnLength.intValue * _labelSpaceHeight * PROPERTY_SPACE;
    }
}


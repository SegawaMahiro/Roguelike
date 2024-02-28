using BehaviorTree;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace BehaviorTreeEditor
{
    [CustomEditor(typeof(BehaviorTreeData))]
    internal class BehaviorTreeInspector : Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            base.OnInspectorGUI();

            if (GUILayout.Button("BehaviorTreeを開く")) {
                BehaviorTreeData runner = (BehaviorTreeData)target;
                BehaviorTreeGraphWindow window = CreateInstance<BehaviorTreeGraphWindow>();
                window.OnOpen(runner);
            }
            if (GUILayout.Button("選択中のNodeのScriptを開く")) {
                BehaviorTreeData runner = (BehaviorTreeData)target;
                string className = runner.SelectingNode.GetType().Name;
                string[] guids = AssetDatabase.FindAssets("t:Script " + className);
                if (guids.Length > 0) {
                    foreach (string guid in guids) {
                        string scriptPath = AssetDatabase.GUIDToAssetPath(guid);
                        Object scriptAsset = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(Object));
                        MonoScript script = scriptAsset as MonoScript;
                        if (script != null && script.GetClass() != null && script.GetClass().Name == className) {
                            Selection.activeObject = scriptAsset;
                            return;
                        }
                    }
                }
                else {
                    UnityEngine.Debug.LogError("スクリプトが見つかりませんでした");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

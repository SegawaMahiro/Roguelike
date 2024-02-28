using BehaviorTree;
using System;
using UnityEditor;
namespace BehaviorTreeEditor
{
    public class BehaviorTreeGraphWindow : EditorWindow
    {
        private BehaviorTreeGraphView _graphView;
        private BehaviorTreeData _data;

        public BehaviorTreeGraphView GraphView => _graphView;
        public BehaviorTreeData Data {get { return _data;} set { _data = value; } }

        public void Open(BehaviorTreeData data) {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            _data = data;
            _graphView = new BehaviorTreeGraphView(this);

            rootVisualElement.Add(_graphView);
            Show();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change) {
            this?.Close();
        }

        private void OnLostFocus() => _graphView.SaveGraph();

        public void OnOpen(BehaviorTreeData treeData) {
            if (HasOpenInstances<BehaviorTreeGraphWindow>()) {
                var window = GetWindow<BehaviorTreeGraphWindow>(treeData.name, typeof(SceneView));

                if (window._data?.GetInstanceID() == treeData.GetInstanceID()) {
                    window.Focus();
                    return;
                }

                window.Open(treeData);
                window.titleContent.text = treeData.name;
                window.Focus();
                return;
            }
            else {
                var window = GetWindow<BehaviorTreeGraphWindow>(treeData.name, typeof(SceneView));
                window.Open(treeData);
                return;
            }
        }
        private void OnInspectorUpdate() {
            _graphView?.NodeUpdate();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using BehaviorTree;

namespace BehaviorTreeEditor
{
    internal class BehaviorTreeNodeSearch : ScriptableObject, ISearchWindowProvider
    {
        private BehaviorTreeGraphView _graphView;
        private BehaviorTreeGraphWindow _window;

        public void Initialize(BehaviorTreeGraphView graphView,BehaviorTreeGraphWindow window) {
            _graphView = graphView;
            _window = window;
        }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context) {
            var entries = new List<SearchTreeEntry> {
                new SearchTreeGroupEntry(new GUIContent("Create Node"))
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {

                    // rootnode以外のnodeを選択肢に追加
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BehaviorTreeNode)) && type != typeof(Root)) {

                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name)) { level = 1, userData = type });
                    }
                }
            }

            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context) {
            var type = entry.userData as Type;

            BehaviorTreeNode node = Activator.CreateInstance(type) as BehaviorTreeNode;
            var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
            var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

            node.NodePosition = localMousePosition;

            _graphView.CreateNodeView(_window.Data.CreateNode(node.GetType(),localMousePosition));

            return true;
        }
    }
}

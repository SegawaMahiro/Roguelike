using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;

namespace BehaviorTreeEditor
{
    internal class BehaviorTreeSaveLoadManager
    {
        private BehaviorTreeGraphWindow _window;
        private BehaviorTreeGraphView _view;

        public BehaviorTreeSaveLoadManager(BehaviorTreeGraphWindow window, BehaviorTreeGraphView view) {
            _window = window;
            _view = view;
        }
        /// <summary>
        /// GraphviewのNode情報を保存
        /// </summary>
        internal void SaveGraph() {
            Dictionary<string, BehaviorTreeNode> nodeDictionary = new Dictionary<string, BehaviorTreeNode>();

            // ノードをguidと一致させる
            foreach (BehaviorTreeGraphNode targetNode in _view.nodes) {
                BehaviorTreeNode currentNode = _window.Data.Nodes.FirstOrDefault(node => node.Guid == targetNode.Guid);

                // ノードが存在する場合追加
                if (currentNode is not null) { nodeDictionary.Add(targetNode.Guid, currentNode); }
            }
            SortChildrenByHorizontal(nodeDictionary);

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(_window.Data);
        }

        /// <summary>
        /// 開いているデータを読み取ってGraphへ反映
        /// </summary>
        internal void LoadGraph() {
            Dictionary<string, BehaviorTreeGraphNode> nodeDictionary = new Dictionary<string, BehaviorTreeGraphNode>();

            foreach (var nodeData in _window.Data.Nodes) {
                // windowへnodeを追加
                BehaviorTreeGraphNode nodeView = new BehaviorTreeGraphNode(nodeData, _view);
                nodeDictionary.Add(nodeData.Guid, nodeView);
                _view.AddElement(nodeView);

                if (nodeData.Children is null) { nodeData.Children = new List<BehaviorTreeNode>(); }
            }

            foreach (var nodeData in _window.Data.Nodes) {
                CreateNodeViewEdge(nodeDictionary, nodeData);
            }
        }

        /// <summary>
        /// 実行するNodeの順を左からソートする
        /// </summary>
        private void SortChildrenByHorizontal(Dictionary<string, BehaviorTreeNode> nodeDictionary) {
            // ノードを水平方向にソートする
            var sortedNodes = _view.nodes.OrderBy(node => node.GetPosition().position.x);

            foreach (BehaviorTreeGraphNode targetNode in sortedNodes) {
                if (nodeDictionary.TryGetValue(targetNode.Guid, out var currentNode)) {
                    // 子ノードの再設定を行う
                    currentNode.Children.Clear();
                    SetChildrenFromPorts(nodeDictionary, targetNode, currentNode);

                    // ノードの位置を更新する
                    Vector2 rect = targetNode.GetPosition().position;
                    currentNode.NodePosition = rect;
                }
            }
        }

        /// <summary>
        /// PortのつながっているNodeを子にする
        /// </summary>
        private void SetChildrenFromPorts(Dictionary<string, BehaviorTreeNode> nodeDictionary, BehaviorTreeGraphNode targetNode, BehaviorTreeNode currentNode) {
            var outputPort = targetNode.outputContainer.Children().OfType<Port>().FirstOrDefault();
            if (outputPort is null) return;
            // 左にあるものから順に接続を保存
            var sortedConnections = outputPort.connections.OrderBy(edge => edge.input.node.GetPosition().position.x);

            // edgeに接続されているすべてのnodeを子に追加
            foreach (Edge edge in sortedConnections) {
                BehaviorTreeGraphNode connectedNode = edge.input.node as BehaviorTreeGraphNode;

                // 子が存在する場合のみ追加する
                if (nodeDictionary.TryGetValue(connectedNode?.Guid, out var connectedBehaviorTreeNode)) {
                    currentNode.Children.Add(connectedBehaviorTreeNode);
                }
            }
        }
        /// <summary>
        /// 子ノードへ向けてのEdgeを作成
        /// </summary>
        private void CreateNodeViewEdge(Dictionary<string, BehaviorTreeGraphNode> nodeDictionary, BehaviorTreeNode nodeData) {
            // 出力ポートを取得
            var outputPort = nodeDictionary.GetValueOrDefault(nodeData.Guid)?.outputContainer.Children().OfType<Port>().FirstOrDefault();
            if (outputPort is null) return;

            foreach (var childNodeData in nodeData.Children) {
                // 子ノードを取得
                if (!nodeDictionary.TryGetValue(childNodeData.Guid, out var childNodeView)) continue;

                // 入力ポートを取得
                var inputPort = childNodeView.inputContainer.Children().OfType<Port>().FirstOrDefault();
                if (inputPort is null) continue;

                // エッジを作成して追加
                var edge = new Edge { output = outputPort, input = inputPort };
                edge.output.Connect(edge);
                edge.input.Connect(edge);
                _view.AddElement(edge);
            }
        }
    }
}

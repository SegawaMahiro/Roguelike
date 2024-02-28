using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BehaviorTree.BehaviorTreeNode;

namespace BehaviorTree
{
    [RequireComponent(typeof(BehaviorTreeBlackBoard))]

    public class BehaviorTreeData : MonoBehaviour
    {

        // Field
        [SerializeField] NodeState _treeStatue = NodeState.Running;
        [Header("──────────────────────────────")]
        [SerializeReference] BehaviorTreeNode _selectingNode;
        [Header("──────────────────────────────")]
        [SerializeReference] List<BehaviorTreeNode> _nodes = new();
        [SerializeReference] BehaviorTreeNode _root;

        private BehaviorTreeNode _runningNode;
        private BehaviorTreeBlackBoard _blackBoard;
        public BehaviorTreeNode SelectingNode => _selectingNode;
        // Properties
        public BehaviorTreeBlackBoard BlackBoard { get { return _blackBoard; } }
        public BehaviorTreeNode Root { get { return _root; } }
        public NodeState TreeStatue { get { return _treeStatue; } }
        public List<BehaviorTreeNode> Nodes { get { return _nodes; } }

        private void Awake() {
            _runningNode = _root;
            _blackBoard = GetComponent<BehaviorTreeBlackBoard>();
        }
        private void Update() {
            StartBehaviorTree();
            if (_treeStatue == NodeState.Running) return;

            _treeStatue = NodeState.Running;
            if (_runningNode != _root) {
                _runningNode = _root;
                return;
            }
            Root.Update();
        }

        private void StartBehaviorTree() {
            if (_treeStatue == NodeState.Running) {
                _treeStatue = _runningNode.Execute();
            }
        }
        internal void SetRunningNode(BehaviorTreeNode node) {
            if (_runningNode == _root) {
                _runningNode = node;
            }
        }

        // Editorからの操作

        /// <summary>
        /// graphview内のnodeと同じ情報を持つnodeの作成
        /// </summary>
        /// <param name="type">作成するnodeの型</param>
        /// <param name="position">生成位置</param>
        /// <returns></returns>
        public BehaviorTreeNode CreateNode(Type type, Vector2 position) {
            // 派生クラスのインスタンスを生成
            BehaviorTreeNode node = Activator.CreateInstance(type) as BehaviorTreeNode;

            // 親ノードは一つのみ存在可能
            if (type == typeof(Root)) {
                _root = node;
            }

            // Nodeの所有者をこのComponentに設定
            node.RootTree = this;
            node.Name = type.Name;
            node.Guid = Guid.NewGuid().ToString();
            node.NodePosition = position;
            _nodes.Add(node);
            return node;
        }

        /// <summary>
        /// 消されたgraphnodeのguidと一致するnodeを削除
        /// </summary>
        /// <param name="guid">消去するnodeのguid</param>
        public void DeleteNode(string guid) {
            BehaviorTreeNode targetNode = _nodes.FirstOrDefault(node => node.Guid == guid);

            Nodes?.Remove(targetNode);
        }

        public void SetNodePosition(string guid, Vector2 position) {
            BehaviorTreeNode targetNode = _nodes.FirstOrDefault(node => node.Guid == guid);

            if (targetNode is not null) {
                targetNode.NodePosition = position;
            }
        }
        public void SetSelectingNode(string guid) {
            BehaviorTreeNode targetNode = _nodes.FirstOrDefault(node => node.Guid == guid);
            if (targetNode is not null) {
                _selectingNode = targetNode;
            }
        }
    }
}

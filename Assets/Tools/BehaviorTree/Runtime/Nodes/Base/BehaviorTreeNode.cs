using System;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorTree
{

    [System.Serializable]
    public abstract class BehaviorTreeNode
    {
        // Fields
        public enum NodeState
        {
            Success,
            Failure,
            Running
        }
        [SerializeField] string _name;
        [SerializeField] bool _breakpoint = false;

        [SerializeField, HideInInspector] BehaviorTreeData _rootTree;
        [SerializeField] NodeState _state = NodeState.Running;
        [SerializeField, HideInInspector] string _guid;
        [SerializeField, HideInInspector] Vector2 _nodePosition;
        [SerializeReference, HideInInspector] List<BehaviorTreeNode> _children = new();

        private bool _isRunning = false;
        private bool _isAwake = true;


        // Properties
        public abstract string Description { get; }

        public string Name { get { return _name; } set { _name = value; } }
        public BehaviorTreeData RootTree { get { return _rootTree; } set { _rootTree = value; } }
        public string Guid { get { return _guid; } set { _guid = value; } }
        public Vector2 NodePosition { get { return _nodePosition; } set { _nodePosition = value; } }
        public NodeState State { get { return _state; } set { _state = value; } }
        public bool Breakpoint { get { return _breakpoint; } set { _breakpoint = value; } }
        public bool IsRunning { get { return _isRunning; } }
        // 現在のnodeが完了時次に実行するnodeのリスト
        public List<BehaviorTreeNode> Children { get { return _children; } set { _children = value; } }


        /// <summary>
        /// treeが更新された際の処理
        /// </summary>
        /// <returns></returns>
        public NodeState Execute() {
            if (_isAwake) {
                OnAwake();
                _isAwake = false;
            }
            if (!_isRunning) {
                OnEnter();
                _isRunning = true;
            }
            // breakpointがついている場合editorを中断
            if (_breakpoint) {
                Debug.Break();
            }
            // 現在のstateを実行
            _state = OnExecute();
            // 完了または失敗時そのnodeを抜ける
            if (_state != NodeState.Running) {
                Exit();
            }
            // 最後に実行されたrunning nodeから次の実行を行う
            //else {
            //    RootTree.SetRunningNode(this);
            //}
            return _state;
        }
        public void Update() {
            if (_children.Count > 0) {
                for (int i = 0; i < _children.Count; i++)
                    _children[i].Update();
            }
            _isRunning = false;
        }
        protected void Exit() {
            OnExit();
        }
        /// <summary>
        /// tree初回起動時の処理
        /// </summary>
        protected virtual void OnAwake() { }
        /// <summary>
        /// 現在のnodeが更新されるたびの実行処理
        /// </summary>
        protected virtual void OnEnter() { }
        /// <summary>
        /// 現在のnodeが完了もしくは失敗した際の処理
        /// </summary>
        protected virtual void OnExit() { }
        /// <summary>
        /// nodeの状態を返す
        /// </summary>
        /// <returns></returns>
        protected abstract NodeState OnExecute();
    }

}


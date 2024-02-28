using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class Sequencer : Composite
    {
        public override string Description => "すべての子ノードを順番に評価する";
        protected int _nodeCount;
        protected override void OnEnter() {
            _nodeCount = 0;
        }
        protected override NodeState OnExecute() {
            var child = Children[_nodeCount];
            switch (child.Execute()) {
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failure:
                    return NodeState.Failure;
                case NodeState.Success:
                    _nodeCount++;
                    break;
            }
            return _nodeCount == Children.Count ? NodeState.Success : NodeState.Running;
        }
    }
}

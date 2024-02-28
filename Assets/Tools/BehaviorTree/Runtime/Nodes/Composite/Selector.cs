using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class Selector : Composite
    {
        public override string Description => "子ノードで最初に成功したものを実行する";
        private int _nodeIndex;

        protected override void OnEnter() {
            _nodeIndex = 0;
        }

        protected override NodeState OnExecute() {
            for (; _nodeIndex < Children.Count; _nodeIndex++) {
                var childState = Children[_nodeIndex].Execute();
                switch (childState) {
                    case NodeState.Success:
                        // 成功した場合はSelectorも成功を返す
                        return NodeState.Success;
                    case NodeState.Running:
                        // 実行中の場合はそのまま実行中を返す
                        return NodeState.Running;
                    case NodeState.Failure:
                        // 失敗した場合は次の子ノードを評価する
                        continue;
                    default:
                        // その他の状態の場合は例外をスローする
                        throw new InvalidOperationException("不明なノードの状態です");
                }
            }

            // 全ての子ノードが失敗した場合はSelectorも失敗を返す
            return NodeState.Failure;
        }
    }
}

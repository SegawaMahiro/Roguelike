using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree
{
    [Serializable]
    public class Parallel : Composite
    {
        public override string Description => "";

        protected override NodeState OnExecute() {
            bool isAnySuccess = true;
            bool isAnyRunning = false;

            foreach (var child in Children) {
                switch (child.Execute()) {
                    case NodeState.Success:
                        break;
                    case NodeState.Failure:
                        return NodeState.Failure;
                    case NodeState.Running:
                        isAnyRunning = true;
                        break;
                }
            }
            if (isAnySuccess) {
                return NodeState.Success;
            }
            if (isAnyRunning) {
                return NodeState.Running;
            }
            return NodeState.Failure;
        }
    }
}

using System;

namespace BehaviorTree
{
    internal class Observer : Decorator
    {
        public override string Description => "";

        protected override NodeState OnExecute() {
            return NodeState.Success;
        }
    }
}

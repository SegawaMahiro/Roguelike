using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public class Root : BehaviorTreeNode,IOutputtable
    {
        public override string Description => "Treeの開始地点";

        public IOutputtable.OutputType PortOutputType => IOutputtable.OutputType.Single;

        protected override NodeState OnExecute() {
            foreach (var child in Children) {
                switch (child.Execute()) {
                    case NodeState.Running:
                        return NodeState.Running;
                    case NodeState.Failure:
                        return NodeState.Failure;

                    case NodeState.Success:
                        break;
                }
                return NodeState.Success;
            }
            return NodeState.Failure;
        }
    }
}

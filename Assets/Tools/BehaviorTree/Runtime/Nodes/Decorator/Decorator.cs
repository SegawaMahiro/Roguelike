namespace BehaviorTree
{
    public abstract class Decorator : BehaviorTreeNode, IInputtable, IOutputtable
    {
        public IOutputtable.OutputType PortOutputType => IOutputtable.OutputType.Single;

        protected BehaviorTreeNode GetChild() {
            return Children[0];
        }
    }
}

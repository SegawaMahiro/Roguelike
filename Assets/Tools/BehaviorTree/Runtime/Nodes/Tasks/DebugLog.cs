using UnityEngine;

namespace BehaviorTree
{
    public class DebugLog : Task
    {
        public override string Description => "";

        [SerializeField] string _blackboardKey;
        protected override NodeState OnExecute() {
            var variable = RootTree.BlackBoard.GetVariable<FloatVariable>(_blackboardKey);
            Debug.Log(variable.Value);
            return NodeState.Success;
        }
    }
}

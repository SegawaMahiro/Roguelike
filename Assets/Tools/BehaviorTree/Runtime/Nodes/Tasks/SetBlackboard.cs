using UnityEngine;

namespace BehaviorTree
{
    public class SetBool : Task
    {
        public override string Description => "";

        [SerializeField] string _blackboardKey;
        [SerializeField] bool _value;

        BoolVariable _var;
        protected override void OnAwake() {
            _var = RootTree.BlackBoard.GetVariable<BoolVariable>(_blackboardKey);
        }
        protected override NodeState OnExecute() {
            _var.Value = _value;
            return NodeState.Success;
        }
    }
}

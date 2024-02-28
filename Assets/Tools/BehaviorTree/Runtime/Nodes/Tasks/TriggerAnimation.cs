using UnityEngine;

namespace BehaviorTree
{
    internal class TriggerAnimation : Task
    {
        public override string Description => "";

        [SerializeField] string _triggerName;
        private Animator _animator;

        protected override void OnAwake() {
            RootTree.gameObject.TryGetComponent(out _animator);
        }

        protected override NodeState OnExecute() {
            _animator.SetTrigger(_triggerName);
            return NodeState.Success;
        }
    }
}

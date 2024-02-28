using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class WaitCurrentAnim : Task
    {
        public override string Description => "";

        private Animator _animator;
        private float _startTime;
        private float _animationLifetime;
        protected override void OnAwake() {
            RootTree.gameObject.TryGetComponent(out _animator);
        }
        protected override void OnEnter() {
            _animationLifetime = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            _startTime = Time.time;
        }

        protected override NodeState OnExecute() {
            if (Time.time - _startTime > _animationLifetime) {
                return NodeState.Success;
            }
            return NodeState.Running;
        }
    }
}

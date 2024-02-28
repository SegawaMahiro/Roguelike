using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Wait : Task
    {
        public override string Description => "";
            

        [SerializeField] float _duration = 1f;
        private float _startTime;

        protected override void OnEnter() {
            _startTime = Time.time;
        }
        protected override NodeState OnExecute() {
            if (Time.time - _startTime > _duration) {
                return NodeState.Success;
            }
            return NodeState.Running;
        }
    }
}

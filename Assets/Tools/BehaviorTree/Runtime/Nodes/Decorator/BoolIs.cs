using UnityEngine;
using System;

namespace BehaviorTree
{
    public class BoolIs : Decorator
    {
        [Serializable]
        enum CheckBool
        {
            True,
            False
        }
        public override string Description => "";
        [SerializeField] BoolReference _value;
        [SerializeField] CheckBool _check;
        protected override NodeState OnExecute() {

            var b = _check switch {
                CheckBool.True => _value.Value == true ? true : false,
                CheckBool.False => _value.Value == false ? true : false,
                _ => throw new Exception($"compare failed")
            };
            if (b) {
                return GetChild().Execute();
            }
            return NodeState.Failure;
        }
    }
}
using UnityEngine;
using System;

namespace BehaviorTree
{
    public class Compare : Decorator
    {
        public override string Description => "";
        [SerializeField] CompareFloat _comapre;

        protected override NodeState OnExecute() {
            if (_comapre.Compare()) {
                return GetChild().Execute();
            }
            else {
                return NodeState.Running;
            }
        }
    }
    [Serializable]
    public class CompareFloat
    {
        [SerializeField] FloatReference _leftValue;
        [SerializeReference, SerializeReferenceDropdown] IOperator _operator;
        [SerializeField] FloatReference _rightValue;

        public bool Compare() => _operator.Compare(_leftValue.Value, _rightValue.Value);
    }
}
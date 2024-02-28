using UnityEngine;
using System;

namespace BehaviorTree
{
    public class LowerDistance : Decorator {
        public override string Description => "";
        [SerializeField] float _distance;
        [SerializeField] VariableType _type = VariableType.Vector3;
        [SerializeField] TransformReference _targetTransform;
        [SerializeField] Vector3Reference _targetPos;

        private float _sqrDistance;
        protected override NodeState OnExecute() {

            _sqrDistance = _type switch {
                VariableType.Transform => (RootTree.transform.position - _targetTransform.Value.position).sqrMagnitude,
                VariableType.Vector3 => (RootTree.transform.position - _targetPos.Value).sqrMagnitude,
                _ => throw new Exception($"{_type} はこのノードでサポートされていません")
            };
            if (_sqrDistance < _distance * _distance) {
                return GetChild().Execute();
            }
            return NodeState.Failure;
        }
    }
}
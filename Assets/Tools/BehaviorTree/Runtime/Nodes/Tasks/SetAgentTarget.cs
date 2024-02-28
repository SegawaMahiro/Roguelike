using UnityEngine;
using UnityEngine.AI;
using System;

namespace BehaviorTree
{
    public class SetAgentTarget : Task
    {
        public override string Description => "";
        [Header("TargetPosition")]
        [SerializeField] VariableType _type = VariableType.Vector3;
        [SerializeField] TransformReference _targetPosition;
        [SerializeField] Vector3Reference _targetPos;

        private NavMeshAgent _agent;


        protected override void OnAwake() {
            RootTree.gameObject.TryGetComponent(out _agent);  
        }
        protected override NodeState OnExecute() {
            _agent.destination = _type switch {
                VariableType.Transform => _targetPosition.Position,
                VariableType.Vector3 => _targetPos.Value,
                _ => throw new Exception($"{_type} はこのノードでサポートされていません")
            };
            return NodeState.Success;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviorTree
{
    public class LookTarget : Task
    {
        public override string Description => "";


        [SerializeField] Transform _target;
        [SerializeField] float _rotationAngle = 5;
        [SerializeField] float _rotationSpeed = 20f; // 回転速度

        private Vector3 _initialRotation;

        protected override void OnEnter() {
            _initialRotation = RootTree.transform.eulerAngles;
        }

        protected override NodeState OnExecute() {
            if (_target != null) {
                Vector3 targetDirection = _target.position - RootTree.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                float newYRotation = targetRotation.eulerAngles.y + _rotationAngle;
                Quaternion newRotation = Quaternion.Euler(_initialRotation.x, newYRotation, _initialRotation.z);
                RootTree.transform.rotation = Quaternion.RotateTowards(RootTree.transform.rotation, newRotation, _rotationSpeed * Time.deltaTime);
                return NodeState.Success; 
            }
            else {
                return NodeState.Failure;
            }
        }
    }
}

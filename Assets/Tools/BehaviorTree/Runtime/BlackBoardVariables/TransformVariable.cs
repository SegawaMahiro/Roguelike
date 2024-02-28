using System;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class TransformVariable : Variable<Transform>
    {
        protected override bool Equals(Transform a, Transform b) {
            return a == b;
        }
    }
    [Serializable]
    public class TransformReference : VariableViewer<TransformVariable, Transform>,IPositionReference
    {
        public Transform Value {
            get { return (_isKeyInput) ? GetVariable().Value : _rawValue; }
            set {
                if (_isKeyInput) {
                    _rawValue = value;
                }
                else {
                    GetVariable().Value = value;
                }
            }
        }

        public Vector3 Position => Value.position;
    }
}
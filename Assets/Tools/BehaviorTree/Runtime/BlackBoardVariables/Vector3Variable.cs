using System;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class Vector3Variable : Variable<Vector3>
    {
        protected override bool Equals(Vector3 a, Vector3 b) {
            return a == b;
        }
    }
    [Serializable]
    public class Vector3Reference : VariableViewer<Vector3Variable, Vector3>,IPositionReference
    {
        public Vector3 Value {
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

        public Vector3 Position => Value;
    }
}

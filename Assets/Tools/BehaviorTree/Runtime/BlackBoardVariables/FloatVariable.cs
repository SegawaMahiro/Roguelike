using System;

namespace BehaviorTree
{
    [Serializable]
    public class FloatVariable : Variable<float>
    {
        protected override bool Equals(float a, float b) {
            return a == b;
        }
    }
    [Serializable]
    public class FloatReference : VariableViewer<FloatVariable, float>
    {
        public float Value {
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
    }
}
using System;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public enum VariableType
    {
        Float,
        Transform,
        Vector3,
        Bool
    }
    [Serializable]
    public abstract class BlackboardVariable
    {
        [SerializeField] protected string _key;

        internal string Key => _key;
    }
    public abstract class Variable<T> : BlackboardVariable
    {
        [SerializeField] protected T _value = default;

        protected abstract bool Equals(T val1, T val2);
        public event Action<T> OnValueChanged;

        public T Value {
            get { return _value; }
            set {
                if (!Equals(_value, value)) {
                    _value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }
    }
    [Serializable]
    public class VariableViewer<T, U> : BaseVariableViewer where T : BlackboardVariable
    {
        protected T _value = null;
        [SerializeField] protected U _rawValue = default;

        public T GetVariable() {
            if (_value != null) {
                return _value;
            }
            if (_blackboard is null || string.IsNullOrEmpty(_key)) {
                return null;
            }
            _value = _blackboard.GetVariable<T>(_key);
            return _value;
        }
    }
    [Serializable]
    public abstract class BaseVariableViewer
    {
        [SerializeField] protected bool _isKeyInput = false;
        [SerializeField] protected BehaviorTreeBlackBoard _blackboard;
        [SerializeField] protected string _key;

        public virtual bool IsKeyInput { get { return _isKeyInput; } }
    }
}

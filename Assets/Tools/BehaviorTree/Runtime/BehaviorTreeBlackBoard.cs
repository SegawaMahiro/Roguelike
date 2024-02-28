using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class BehaviorTreeBlackBoard : MonoBehaviour
    {

        [SerializeReference, SerializeReferenceDropdown] BlackboardVariable[] _blackboards;
        private Dictionary<string, BlackboardVariable> _dictionary = new();

        private void Awake() {
            _dictionary.Clear();
            for (int i = 0; i < _blackboards.Length; i++) {
                BlackboardVariable variable = _blackboards[i];
                _dictionary.Add(variable.Key, variable);
            }
        }
        public T GetVariable<T>(string key) where T : BlackboardVariable {
            return (_dictionary.TryGetValue(key, out BlackboardVariable value)) ? (T)value : null;
        }
    }
}
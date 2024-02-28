using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class BoolVariable : Variable<bool>
    {
        protected override bool Equals(bool a, bool b) {
            return a == b;
        }
    }
    [Serializable]
    public class BoolReference : VariableViewer<BoolVariable, bool>
    {
        public bool Value {
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

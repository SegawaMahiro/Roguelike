using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviorTree
{
    public class ToggleGameObject : Task
    {
        public override string Description => "";

        [SerializeField] GameObject _targetObject;
        [SerializeField] bool _toggle;
        protected override NodeState OnExecute() {
            _targetObject.SetActive(_toggle);
            return NodeState.Success;
        }
    }
}

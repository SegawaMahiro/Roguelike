using UnityEngine;

namespace BehaviorTree
{
    public class SetRandomPosition : Task
    {
        public override string Description => "";
        [Header("TargetPosition")]
        [SerializeField] string _key;
        [SerializeField] Vector3 _center;
        [SerializeField] float _radius;

        private Vector3Variable _vector3Var;

        protected override void OnAwake() {
            _vector3Var = RootTree.BlackBoard.GetVariable<Vector3Variable>(_key);
        }
        protected override NodeState OnExecute() {
            var randX = Random.Range(-_radius, _radius);
            var randY = Random.Range(-_radius, _radius);
            var pos = _center + new Vector3(randX, 0, randY);
            _vector3Var.Value = pos;
            return NodeState.Success;
        }
    }
}

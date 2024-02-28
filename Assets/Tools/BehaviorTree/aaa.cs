using BehaviorTree;
using UnityEngine;

public class aaa : MonoBehaviour
{
   [SerializeField] BehaviorTreeBlackBoard _blackboard;
    private void Start() {
        var f = _blackboard.GetVariable<FloatVariable>("Test");
        f.Value = 100;
    }
}

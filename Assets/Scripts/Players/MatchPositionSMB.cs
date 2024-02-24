using UnityEngine;

namespace Roguelike.Players
{
    internal class MatchPositionSMB : StateMachineBehaviour
    {
    }
    public interface IMatchTarget {
        Vector3 TargetPosition { get;}
    }
}

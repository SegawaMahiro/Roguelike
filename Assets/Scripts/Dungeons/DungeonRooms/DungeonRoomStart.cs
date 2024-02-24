using UnityEngine;

namespace Roguelike.Dungeons
{
    [System.Serializable]
    public sealed class DungeonRoomStart : DungeonRoom1x1
    {
        public override bool IsSingleGate => true;
    }
}

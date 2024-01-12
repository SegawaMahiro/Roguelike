using UnityEngine;

namespace Assets.Scripts.Dungeons
{
    [CreateAssetMenu(fileName = "new 1x1Room", menuName = "ScriptableObjects/DungeonTiles/1x1")]
    internal class DungeonRoom1x1 : RoomData
    {
        private bool[,] _shape = new bool[1, 1] {
            {true }
        };
        public sealed override bool[,] Shape => _shape;
    }
}

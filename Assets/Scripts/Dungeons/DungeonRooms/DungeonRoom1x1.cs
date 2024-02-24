using UnityEngine;

namespace Roguelike.Dungeons
{
    public class DungeonRoom1x1 : RoomData
    {

        private bool[,] _shape = new bool[1, 1] {
            {true }
        };
        public sealed override bool[,] Shape => _shape;

    }
}

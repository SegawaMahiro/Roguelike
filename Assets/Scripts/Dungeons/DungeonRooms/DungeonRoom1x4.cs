using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Dungeons
{
    [CreateAssetMenu(fileName = "new 1x4room", menuName = "ScriptableObjects/DungeonTiles/1x4")]
    internal class DungeonRoom1x4 : RoomData
    {
        private bool[,] _shape = new bool[4, 1] {
            {true},
            {true},
            {true},
            {true}
        };
        public override bool[,] Shape => _shape;
    }
}

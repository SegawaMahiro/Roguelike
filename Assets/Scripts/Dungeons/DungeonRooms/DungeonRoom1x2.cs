using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Dungeons
{
    [CreateAssetMenu(fileName = "new 1x2room", menuName = "ScriptableObjects/DungeonTiles/1x2")]
    internal class DungeonRoom1x2 : RoomData
    {
        private bool[,] _shape = new bool[2, 1] {
            {true},
            {true}
        };
        public override bool[,] Shape => _shape;
    }
}

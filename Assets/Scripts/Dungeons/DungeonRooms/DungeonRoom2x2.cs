using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Dungeons
{
    [CreateAssetMenu(fileName = "new 2x2room", menuName = "ScriptableObjects/DungeonTiles/2x2")]
    internal class DungeonRoom2x2 : RoomData
    {
        private bool[,] _shape = new bool[2, 2] {
            {true,true},
            {true,true}
        };
        public override bool[,] Shape => _shape;
    }
}

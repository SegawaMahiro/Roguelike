using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roguelike.Dungeons
{
    internal class DungeonRoom1x3 : RoomData
    {
        private bool[,] _shape = new bool[3, 1] {
            {true},
            {true},
            {true}
        };
        public override bool[,] Shape => _shape;
    }
}

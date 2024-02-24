using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roguelike.Dungeons
{
    internal class DungeonRoomL : RoomData
    {
        private bool[,] _shape = new bool[2, 2] {
            {true,false },
            {true,true }
        };
        public override bool[,] Shape => _shape;
    }
}

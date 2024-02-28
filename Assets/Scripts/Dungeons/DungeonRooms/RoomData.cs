using UnityEngine;
using System.Collections.Generic;

namespace Roguelike.Dungeons
{
    public abstract class RoomData {
        public abstract bool[,] Shape { get; }
        public virtual bool IsSingleGate {  get; }

       private string _guid;
        private int _simulatedCount;
        private Vector2Int _arrayPosition;
        public string GUID {  get { return _guid; } set { _guid = value; } }
        public Vector2Int ArrayPosition { get { return _arrayPosition; } set { _arrayPosition = value; } }
        public int SimulatedCount { get { return _simulatedCount; } set { _simulatedCount = value; } }
    }
}

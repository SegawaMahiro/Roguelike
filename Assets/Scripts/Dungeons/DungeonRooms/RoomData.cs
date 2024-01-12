using UnityEngine;
using Assets.Scripts.Utils;
using System.Collections.Generic;

namespace Assets.Scripts.Dungeons
{
    internal abstract class RoomData : ScriptableObject {
        public abstract bool[,] Shape { get; }

        [SerializeField] GameObject _roomObject;
        [SerializeField] bool _isSingleGate;

        private string _guid;
        private int _simulatedCount;
        private Vector2Int _arrayPosition;

        public GameObject RoomObject { get { return _roomObject; } set { _roomObject = value; } }
        public string GUID {  get { return _guid; } set { _guid = value; } }
        public Vector2Int ArrayPosition { get { return _arrayPosition; } set { _arrayPosition = value; } }
        public int SimulatedCount { get { return _simulatedCount; } set { _simulatedCount = value; } }
        public bool IsSingleGate {  get { return _isSingleGate; } }

        public GameObject GenerateRoom(Vector3 position ,float distance) {
            if(_roomObject == null) return null;
            // 二次元配列の中心
            var x = Shape.GetLength(0) / 2 * distance / 2;
            var y = Shape.GetLength(1) / 2 * distance / 2;

            var generatePos = new Vector3(position.x+x, position.y, position.z+y);
            GameObject room = Instantiate(_roomObject,generatePos,Quaternion.identity);
            return room;
        }
    }
}

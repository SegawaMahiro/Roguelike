using UnityEngine;

namespace Roguelike.Dungeons
{
    internal class DungeonObjectGenerator
    {
        private int _xSize;
        private int _ySize;

        private RoomData[,] _grid;
        private int _roomSize;
        private DungeonRoomData _roomData;
        public GameObject WallPrefab;
        public GameObject FloorPrefab;

        internal DungeonObjectGenerator(RoomData[,] grid, int roomSize, DungeonRoomData roomData) {
            _grid = grid;
            _xSize = grid.GetLength(0);
            _ySize = grid.GetLength(1);
            _roomSize = roomSize;
            _roomData = roomData;
        }
        internal void SetRoomObjects() {
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    GenerateFloor(x, y);
                    GenerateWalls(x, y);
                }
            }
        }
        private void GenerateFloor(int x, int y) {
            var ground = Object.Instantiate(FloorPrefab);
            ground.transform.position = new Vector3(x * _roomSize, 0, y * _roomSize);
        }
        private void GenerateWalls(int x, int y) {
            // 4辺すべてに処理を行う
            for (int i = 0; i < 4; i++) {
                var wall = Object.Instantiate(WallPrefab);
                // 壁が中央を向くよう回転させる
                wall.transform.rotation = Quaternion.Euler(0, i * 90, 0);

                var direction = wall.transform.forward;
                wall.transform.position = new Vector3(x * _roomSize + direction.x * _roomSize/2, 0, y * _roomSize + direction.z * _roomSize / 2);
            }
        }
        private void GenerateInterior(int x, int y) {
        }
    }
}

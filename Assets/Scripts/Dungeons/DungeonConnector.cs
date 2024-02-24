using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Roguelike.Dungeons
{
    internal class DungeonConnector
    {
        private int _xSize;
        private int _ySize;
        private RoomData[,] _grid;
        private int _roomCollections;
        private int _roomSize;

        internal DungeonConnector(RoomData[,] grid, int roomSize) {
            _grid = grid;
            _xSize = grid.GetLength(0);
            _ySize = grid.GetLength(1);
            _roomSize = roomSize;
        }

        internal RoomData[,] StartConnect(Vector2Int pos) {
            ConnectRooms(pos.x, pos.y);
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    if (_grid[x, y].SimulatedCount == 0) {
                        ConnectRooms(x, y);
                    }
                }
            }
            for (int i = 0; i < _roomCollections; i++) {
                CombineConnects();
            }
            return _grid;
        }

        private void CombineConnects() {
            var roomGroup = _grid.Cast<RoomData>().Where(room => room.SimulatedCount == 1 && !room.IsSingleGate).ToList();
            foreach (var room in roomGroup) {
                var connectedGroup = ConnectRoomGroup(room.ArrayPosition.x, room.ArrayPosition.y);
                if (connectedGroup <= 0) continue;

                var roomGroup2 = _grid.Cast<RoomData>().Where(r => r.SimulatedCount == connectedGroup).ToList();
                foreach (var room2 in roomGroup2) {
                    _grid[room2.ArrayPosition.x, room2.ArrayPosition.y].SimulatedCount = 1;
                }
                break;
            }
        }

        private int ConnectRoomGroup(int startX, int startY) {
            var directions = Enumerable.Range(0, 4).OrderBy(a => Random.value);
            foreach (var direction in directions) {
                int x, y;
                GetDirection(startX, startY, direction, out x, out y);
                if (x < 0 || x >= _xSize || y < 0 || y >= _ySize) continue;
                var room = _grid[x, y];
                var parent = _grid[startX, startY];
                if ((room.SimulatedCount == parent.SimulatedCount) || room.IsSingleGate) continue;
                GetObjectsOnLine(new Vector3(startX * _roomSize, 3, startY * _roomSize), new Vector3(x * _roomSize, 3, y * _roomSize));
                return room.SimulatedCount;
            }
            return 0;
        }

        private void ConnectRooms(int startX, int startY) {
            var directions = Enumerable.Range(0, 4).OrderBy(a => Random.value);
            bool isConnected = false;
            foreach (var direction in directions) {
                int x, y;
                GetDirection(startX, startY, direction, out x, out y);
                if (x < 0 || x >= _xSize || y < 0 || y >= _ySize) continue;
                var room = _grid[x, y];
                var parent = _grid[startX, startY];
                if (room.SimulatedCount != 0 || room.IsSingleGate) continue;
                isConnected = true;
                _grid[startX, startY].SimulatedCount = _roomCollections + 1;
                GetObjectsOnLine(new Vector3(startX * _roomSize, 3, startY * _roomSize), new Vector3(x * _roomSize, 3, y * _roomSize));
                ConnectRooms(x, y);
                break;
            }
            if (!isConnected && _grid[startX, startY].SimulatedCount == 0) {
                _roomCollections++;
                _grid[startX, startY].SimulatedCount = _roomCollections;
            }
        }

        private void GetDirection(int startX, int startY, int direction, out int x, out int y) {
            (x, y) = direction switch {
                0 => (startX, startY + 1),
                1 => (startX, startY - 1),
                2 => (startX - 1, startY),
                3 => (startX + 1, startY),
                _ => throw new System.NotImplementedException(),
            };
        }

        private void GetObjectsOnLine(Vector3 start, Vector3 end) {
            RaycastHit[] hits = Physics.RaycastAll(start, end - start, Vector3.Distance(start, end));
            GameObject[] objects = hits.Select(hit => hit.collider.gameObject).ToArray();
            foreach (var obj in objects) {
                Object.Destroy(obj.gameObject);
            }
        }
    }
}

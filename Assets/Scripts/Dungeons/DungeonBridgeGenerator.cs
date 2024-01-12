using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Dungeons
{
    internal class DungeonBridgeGenerator
    {
        private int _xSize;
        private int _ySize;

        private RoomData[,] _grid;
        private int _roomCollections;
        private int _roomSize;

        internal DungeonBridgeGenerator(RoomData[,] grid ,int roomSize) {
            _grid = grid;
            _xSize = grid.GetLength(0);
            _ySize = grid.GetLength(1);
            _roomSize = roomSize;
        }


        /// <summary>
        /// 部屋の接続を開始
        /// </summary>
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
        /// <summary>
        /// 複数に分かれた接続を結合し一つにする
        /// </summary>
        private void CombineConnects() {
            var roomGroup = new List<RoomData>();
            // 同じ実行時に接続された部屋を取得

            foreach (var room in _grid) {
                if (room.SimulatedCount == 1 && !room.IsSingleGate) {
                    roomGroup.Add(room);
                }
            }

            // 同じ集まりの中の部屋をすべて処理
            foreach (var room in roomGroup) {
                // 隣接する別領域の部屋が存在する場合その部屋の領域を返す
                var connectedGroup = ConnectRoomGroup(room.ArrayPosition.x, room.ArrayPosition.y);
                if (connectedGroup > 0) {

                    var roomGroup2 = new List<RoomData>();
                    // 存在した場合現在領域の部屋をすべて取得
                    foreach (var r in _grid) {
                        if (r.SimulatedCount == connectedGroup) { roomGroup2.Add(r); }
                    }
                    foreach (var room2 in roomGroup2) {
                        _grid[room2.ArrayPosition.x, room2.ArrayPosition.y].SimulatedCount = 1;
                    }

                    break;
                }
            }
        }
        private int ConnectRoomGroup(int startX, int startY) {
            var directions = Enumerable.Range(0, 4).OrderBy(a => Random.value);

            foreach (var direction in directions) {
                (int x, int y) = direction switch {
                    0 => (startX, startY + 1), // 上
                    1 => (startX, startY - 1), // 下
                    2 => (startX - 1, startY), // 左
                    3 => (startX + 1, startY), // 右
                    _ => throw new System.NotImplementedException(),
                };
                // 配列の範囲外
                if (x < 0 || x >= _xSize || y < 0 || y >= _ySize) continue;

                var room = _grid[x, y];
                var parent = _grid[startX, startY];

                if ((room.SimulatedCount == parent.SimulatedCount) || room.IsSingleGate) continue;

             Debug.DrawLine(new Vector3(startX * _roomSize, 0, startY * _roomSize),
                            new Vector3(x * _roomSize, 0, y * _roomSize), Color.red, 200f);
                return room.SimulatedCount;
            }
            return 0;
        }

        private void ConnectRooms(int startX, int startY) {

            var directions = Enumerable.Range(0, 4).OrderBy(a => Random.value);
            bool isConnected = false;

            foreach (var direction in directions) {
                (int x, int y) = direction switch {
                    0 => (startX, startY + 1), // 上
                    1 => (startX, startY - 1), // 下
                    2 => (startX - 1, startY), // 左
                    3 => (startX + 1, startY), // 右
                    _ => throw new System.NotImplementedException(),
                };
                // 配列の範囲外
                if (x < 0 || x >= _xSize || y < 0 || y >= _ySize) continue;

                var room = _grid[x, y];
                var parent = _grid[startX, startY];

                // すでに接続済みか、他からの接続を受けない場合スキップ
                if (room.SimulatedCount != 0 || room.IsSingleGate) continue;

                isConnected = true;
                // 現在存在する部屋の集まりに新しいものを追加
                _grid[startX, startY].SimulatedCount = _roomCollections + 1;

                GetObjectsOnLine(new Vector3(startX * _roomSize, 3, startY * _roomSize),
                                                              new Vector3(x * _roomSize, 3, y * _roomSize));

                ConnectRooms(x, y);
                break;
            }
            // 再帰がすべて終了した際に部屋の塊として追加
            if (!isConnected && _grid[startX, startY].SimulatedCount == 0) {
                _roomCollections++;
                _grid[startX, startY].SimulatedCount = _roomCollections;

            }
        }
        private void GetObjectsOnLine(Vector3 start, Vector3 end) {
            RaycastHit[] hits = Physics.RaycastAll(start, end - start, Vector3.Distance(start, end));

            GameObject[] objects = hits.Select(hit => hit.collider.gameObject).ToArray();

            foreach(var obj in objects) {
                obj.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}
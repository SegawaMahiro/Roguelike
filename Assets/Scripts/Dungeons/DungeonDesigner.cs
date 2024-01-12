using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Dungeons
{
    public class DungeonDesigner : MonoBehaviour
    {
        [SerializeField] int _xSize = 6;
        [SerializeField] int _ySize = 6;
        [SerializeField] DungeonRoomObjects _roomData;
        [SerializeField] int _roomSize = 30;
        private RoomData[,] _grid;

        private void Start() {
            OnStart();
        }
        void OnStart() {
            _roomData = Instantiate(_roomData);
            _grid = new RoomData[_xSize, _ySize];
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    _grid[x, y] = Instantiate(_roomData.SingleRooms[0]);
                }
            }
            Vector2Int startRoomPosition = SetStartRoom();

            SelectShapedRoom();

            GenerateGrid();
            DungeonBridgeGenerator connector = new DungeonBridgeGenerator(_grid,_roomSize);
            _grid = connector.StartConnect(startRoomPosition);
        }
        /// <summary>
        /// 周囲4辺のランダムな位置に開始地点を作成
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private Vector2Int SetStartRoom() {
            // 4面のうちの一つを指定
            var side = UnityEngine.Random.Range(0, 4);

            // 面の中からランダムな位置をスタート位置に変更
            int x, y;
            GetRandomWallSide(side, out x, out y);

            _grid[x, y] = Instantiate(_roomData.StartRoom);
            return new Vector2Int(x, y);
        }

        private void GetRandomWallSide(int side, out int x, out int y) {
            (x, y) = side switch {
                0 => (0, UnityEngine.Random.Range(0, _ySize)), // 上辺
                1 => (_xSize - 1, UnityEngine.Random.Range(0, _ySize)), // 下辺
                2 => (UnityEngine.Random.Range(0, _xSize), 0), // 左辺
                3 => (UnityEngine.Random.Range(0, _xSize), _ySize - 1), // 右辺
                _ => throw new System.NotImplementedException(),
            };
        }

        /// <summary>
        /// 形を選択
        /// </summary>
        private void SelectShapedRoom() {
            var positions = new List<Vector2Int>();
            
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    positions.Add(new Vector2Int(x, y));
                }
            }
            var randomPoint = positions.OrderBy(a => UnityEngine.Random.value);

            foreach (var position in randomPoint) {
                if (_roomData.ShapedRooms.Count < 1) return;
                var randomIndex = UnityEngine.Random.Range(0, _roomData.ShapedRooms.Count);
                var room = _roomData.ShapedRooms[randomIndex];

                if (HasSpaceInRoom(position, room)) {

                    string guid = Guid.NewGuid().ToString();
                    SetShapedRoom(position, room , guid);
                    // 生成済みの部屋は再度扱わない
                    _roomData.ShapedRooms.Remove(room);
                }
            }
        }

        /// <summary>
        /// この形状の部屋を生成する空きがあるかどうか
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        bool HasSpaceInRoom(Vector2Int position, RoomData room) {
            for (int x = 0; x < room.Shape.GetLength(0); x++) {
                for (int y = 0; y < room.Shape.GetLength(1); y++) {

                    Vector2Int setPos = position + new Vector2Int(x, y);
                    if (!room.Shape[x, y]) continue;
                    if (setPos.x > _xSize - 1 || setPos.y > _ySize - 1) {
                        return false;
                    }
                    if (_grid[setPos.x, setPos.y] is DungeonRoomStart ||
                        _grid[setPos.x,setPos.y] is not DungeonRoom1x1) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 形に添った部屋を作成
        /// </summary>
        /// <param name="position"></param>
        /// <param name="room"></param>
        private void SetShapedRoom(Vector2Int position, RoomData room,string guid) {
            for (int x = 0; x < room.Shape.GetLength(0); x++) {
                for (int y = 0; y < room.Shape.GetLength(1); y++) {
                    if (room.Shape[x, y]) {
                        Vector2Int setPos = position + new Vector2Int(x, y);
                        _grid[setPos.x, setPos.y] = Instantiate(room);
                        _grid[setPos.x, setPos.y].GUID = guid;
                        if(x + y > 0) {
                            _grid[setPos.x, setPos.y].RoomObject = null;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 部屋の情報によってオブジェクトを生成
        /// </summary>
        void GenerateGrid() {
            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    Vector3 position = new Vector3(x * _roomSize, 0, y * _roomSize);
                    var room = _grid[x, y];
                    room.ArrayPosition = new Vector2Int(x, y);
                    GameObject roomObj = room.GenerateRoom(position, _roomSize);
                    if (roomObj != null) {
                        roomObj.transform.parent = transform;
                    }
                }
            }
        }
    }
}
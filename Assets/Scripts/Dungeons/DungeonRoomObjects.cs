using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Dungeons
{
    [CreateAssetMenu(fileName = "ScriptableObjects", menuName = "DungeonRoomAsset")]
    public class DungeonRoomObjects : ScriptableObject
    {

        [SerializeField] DungeonRoomStart _startRoom;
        [SerializeField] DungeonRoom1x1 _bossRoom;
        [SerializeField] List<DungeonRoom1x1> _singleRooms;
        [SerializeField] List<RoomData> _shapedRooms;
        [SerializeField] GameObject _gatePrefab;
        [SerializeField] GameObject _wallPrefab;

        internal DungeonRoomStart StartRoom { get { return _startRoom; } }
        internal DungeonRoom1x1 BossRoom {  get { return _bossRoom; } } 
        internal List<DungeonRoom1x1> SingleRooms { get { return _singleRooms; } }
        internal List<RoomData> ShapedRooms { get { return _shapedRooms; } }
        internal GameObject GatePrefab { get { return _gatePrefab; } }
        internal GameObject WallPrefab { get { return _wallPrefab; } }
    }
}

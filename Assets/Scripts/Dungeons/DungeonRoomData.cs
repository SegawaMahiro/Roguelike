using UnityEngine;

namespace Roguelike.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonRoomData", menuName = "ScriptableObjects/DungeonRoomData")]
    internal class DungeonRoomData : ScriptableObject
    {
        [SerializeReference] DungeonRoomStart _startRoom = new();
        public DungeonRoomStart StartRoom { get {  return _startRoom; } }
    }
}

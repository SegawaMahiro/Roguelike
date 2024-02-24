using System.Collections.Generic;
using UnityEngine;

namespace Roguelike.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonData", menuName = "ScriptableObjects/DungeonPartsData")]
    internal class BaseDungeonRoomParts : ScriptableObject
    {
        [SerializeField] List<DungeonPartsData> _grounds;
        [SerializeField] List<DungeonPartsData> _walls;
        public List<DungeonPartsData> Grounds { get {  return _grounds; } }
        public List<DungeonPartsData> Walls { get { return _walls; } }
    }
}

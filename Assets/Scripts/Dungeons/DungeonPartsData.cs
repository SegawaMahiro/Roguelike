using System;
using UnityEngine;

namespace Roguelike.Dungeons
{
    [System.Serializable]
    internal struct DungeonPartsData
    {
        [SerializeField] GameObject _prefabObject;
        [SerializeField,Range(0,1)] float _probability;
        public GameObject PrefabObject { get { return _prefabObject; } }
        public float Probability { get { return _probability; } }

    }
}

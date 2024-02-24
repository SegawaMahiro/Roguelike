using UnityEngine;
using System;

namespace Roguelike.Damages.Hitbox
{
    [Serializable]
    public class CapsuleHitboxShape : IHitboxShape
    {
        [SerializeField] float _radius;
        [SerializeField] LayerMask _layerMask;

        public int Overlap(Vector3 start, Vector3 end, Collider[] results) {
            int overlapCount = Physics.OverlapCapsuleNonAlloc(start, end, _radius, results, _layerMask);
            return overlapCount;
        }

    }
}

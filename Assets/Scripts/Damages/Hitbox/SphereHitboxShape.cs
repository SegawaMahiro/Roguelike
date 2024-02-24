using UnityEngine;
using System;

namespace Roguelike.Damages.Hitbox
{
    [Serializable]
    public class SphereHitboxShape : IHitboxShape
    {
        [SerializeField] float _radius;
        [SerializeField] LayerMask _layerMask;

        public int Overlap(Vector3 start, Vector3 end, Collider[] result) {
            return Physics.OverlapSphereNonAlloc(start, _radius, result, _layerMask);
        }
    }
}

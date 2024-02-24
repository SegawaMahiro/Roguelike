using UnityEngine;
using System;

namespace Roguelike.Damages.Hitbox
{
    public interface IHitboxShape
    {
        int Overlap(Vector3 start, Vector3 end, Collider[] results);
    }
}

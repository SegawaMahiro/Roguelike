using UnityEngine;
using System;

namespace Roguelike.Damages.Hitbox
{
    [Serializable]
    public class BoxHitboxShape : IHitboxShape
    {
        [SerializeField] float _width;
        [SerializeField] float _height;
        [SerializeField] LayerMask _layerMask;

        public int Overlap(Vector3 start, Vector3 end, Collider[] result) {
            Vector3 direction = start - end;
            float distance = direction.magnitude;

            // 2点の中心座標
            Vector3 center = (start + end) / 2f;
            // 角度によって生成されない部分を補うため1.5倍
            Vector3 scale = new Vector3(_width, _height, Mathf.Max(distance, _height) * 1.5f);

            // オブジェクトの移動方向へ作成し、Overlapboxは大きさの半分を扱うため *0.5
            return Physics.OverlapBoxNonAlloc(center, scale * 0.5f, result, Quaternion.LookRotation(direction), _layerMask);
        }
    }
}

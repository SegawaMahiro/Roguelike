using System;
#if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;

namespace BehaviorTree
{
    public class IsVisible : Decorator
    {
        public override string Description => "";
        [Header("Target")]
        [SerializeField] TransformReference _target;
        [Header("Eye")]
        [SerializeField] TransformReference _eye;
        [SerializeField] float _range;
        [Range(0, 360), SerializeField] float _angle = 30f;
        [SerializeField] float _circleRadius = 2f;
        [SerializeField] LayerMask _targetMask;
        [SerializeField] LayerMask _hittableMask;

        private readonly Collider[] _overlap = new Collider[10];
        private readonly Vector3[] _targetPoints = new Vector3[5];

        protected override NodeState OnExecute() {
#if UNITY_EDITOR
            GizmoDrawer.Instance.AddGizmoDrawer(() => {
                if (_eye == null || _target == null) return;

                var eyeForward = _eye.Value.forward;
                var halfAngle = _angle / 2;

                Handles.color = new Color(1f, 0.92f, 0.016f, 0.1f);
                Handles.DrawSolidArc(_eye.Value.position, Vector3.up, Quaternion.Euler(0f, -halfAngle, 0f) * eyeForward, halfAngle * 2, _range);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(_eye.Value.position, eyeForward * 5);
            }, 0f);
#endif

            if (!IsTargetInVision()) {
                return NodeState.Failure;
            }
            return GetChild().Execute();
        }


        private bool IsTargetInVision() {

            var hitColliders = Physics.OverlapSphereNonAlloc(_eye.Value.position, _range, _overlap, _targetMask);

            for (int i = 0; i < hitColliders; i++) {
                var currentTarget = _overlap[i];
                if (currentTarget.gameObject != _target.Value.gameObject) continue; 

                Vector3 directionToCollider = currentTarget.transform.position - _eye.Value.position;

                // 視野の計算
                float dotProduct = Vector3.Dot(_eye.Value.forward.normalized, directionToCollider.normalized);
                float angleToCollider = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

                // targetがangleの範囲内に存在しない場合
                if (angleToCollider > _angle / 2f) continue;

                GetColliderPoints(currentTarget);
                return IsTargetFound();
            }
            return false;
        }

        private bool IsTargetFound() {
            foreach (Vector3 point in _targetPoints) {
                RaycastHit hit;

                if (Physics.Raycast(_eye.Value.position, point - _eye.Value.position, out hit, _range, _hittableMask) && hit.collider.gameObject == _target.Value.gameObject) {
                    Debug.DrawLine(_eye.Value.position, point, Color.green, 0.1f);
                    return true;
                }
                Debug.DrawLine(_eye.Value.position, point, Color.red, 0.1f);
            }
            return false;
        }

        private void GetColliderPoints(Collider target) {
            Bounds targetBounds = target.bounds;

            Vector3 targetCenter = targetBounds.center;

            float halfWidth = targetBounds.extents.x;
            float halfHeight = targetBounds.extents.y;

            Vector3 topPosition = targetCenter + Vector3.up * halfHeight;
            Vector3 bottomPosition = targetCenter - Vector3.up * halfHeight;
            Vector3 leftPosition = targetCenter - Vector3.right * halfWidth;
            Vector3 rightPosition = targetCenter + Vector3.right * halfWidth;

            _targetPoints[0] = targetCenter;
            _targetPoints[1] = topPosition;
            _targetPoints[2] = bottomPosition;
            _targetPoints[3] = leftPosition;
            _targetPoints[4] = rightPosition;
        }
    }
}
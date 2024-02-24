using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] float _range;
    [Range(0, 360),SerializeField] float _angle = 30f;

    private void Update() {
        Force();
    }

    void Force() {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, _range);

        for (int i = 0; i < hitColliders.Length; i++) {
            Vector3 directionToCollider = hitColliders[i].transform.position - transform.position;

            // targetがangleの範囲内に存在するか
            float angleToCollider = Vector3.SignedAngle(transform.forward, directionToCollider, transform.up);

            if (Mathf.Abs(angleToCollider) <= _angle / 2f) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToCollider, out hit)) {
                    if (hit.collider.gameObject == hitColliders[i].gameObject) {
                        // targetが見えている場合緑のライン
                        Debug.DrawRay(transform.position, directionToCollider, Color.green, 0.5f);
                    }
                    else {
                        // 遮られている場合赤のライン
                        Debug.DrawRay(transform.position, directionToCollider, Color.red, 0.5f);
                    }
                }
            }
        }
    }
}

using UnityEngine;
using R3;
using Cinemachine;
using Roguelike.Players;
using Cysharp.Threading.Tasks;

public class LockOnCamera : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Transform _target;

    [SerializeField] Vector3 _defaultDistance;

    [SerializeField] CinemachineVirtualCamera _tpsCamera;
    [SerializeField] CinemachineVirtualCamera _lockonCamera;

    [SerializeField] Transform _lockOnCenter;
    [SerializeField] GameObject _lockonTargetArrow;

    [SerializeField] CinemachineTargetGroup _targetGroup;

    // カメラの視点
    private CinemachinePOV _tpsPOV;
    private CinemachinePOV _lockonPOV;


    private PlayerMove _playerMove;

    private void Awake() {
        Observable.EveryUpdate().Subscribe(_ => UpdateCamera()).AddTo(this);
        _playerMove = _player.GetComponent<PlayerMove>();
        _tpsPOV = _tpsCamera.GetCinemachineComponent<CinemachinePOV>();
        _lockonPOV = _lockonCamera.GetCinemachineComponent<CinemachinePOV>();

        Observable.EveryValueChanged(_playerMove.IsLockOn, x => x.CurrentValue)
            .Subscribe(x => {

                if (IsTargetActive()) {
                    OnLockOn(x);
                    return;
                }
                if (!x) return;
                float targetAngle = _player.eulerAngles.y;
                float currentAngle = _tpsPOV.m_HorizontalAxis.Value;
                RotateToTargetAngleAsync(currentAngle, targetAngle, 0.1f).Forget();


            }).AddTo(this);
    }
    private void OnLockOn(bool toggle) {
        // targetが存在するか
        if (_target != _playerMove.Target) {
            _target = _playerMove.Target;
            // targetを保存してるindex
            _targetGroup.m_Targets[1].target = _target;
        }

        // カメラの角度を前回のカメラと同じにする
        if (toggle) {
            _lockonPOV.m_HorizontalAxis.Value = _tpsPOV.m_HorizontalAxis.Value;
            _lockonPOV.m_VerticalAxis.Value = _tpsPOV.m_VerticalAxis.Value;
        }
        else {
            _tpsPOV.m_HorizontalAxis.Value = _lockonPOV.m_HorizontalAxis.Value;
            _tpsPOV.m_VerticalAxis.Value = _lockonPOV.m_VerticalAxis.Value;
        }
        // カメラの切り替え
        _tpsCamera.enabled = !toggle;
        _lockonCamera.enabled = toggle;


        _lockonTargetArrow.transform.parent = _target;
        _lockonTargetArrow.transform.position = _target.position + new Vector3(0, 1.5f, 0);

        _lockonTargetArrow.SetActive(toggle);
    }


    private void UpdateCamera() {
        // playerとtargetの中心座標を取得
        Vector3 center = (_player.position + _target.position) / 2;
        _lockOnCenter.transform.position = center;
    }
    private bool IsTargetActive() {
        return _target.gameObject.activeSelf &&
               _playerMove.gameObject.activeSelf &&
               _playerMove.Target != null &&
               _target != null;
    }
    private async UniTask RotateToTargetAngleAsync(float startAngle, float targetAngle, float duration) {
        // 最短の角度を計算
        float shortestAngle = Mathf.DeltaAngle(startAngle, targetAngle);

        float elapsedTime = 0f;
        // duration秒かけて回転
        while (elapsedTime < duration) {
            float newAngle = Mathf.Lerp(0f, shortestAngle, elapsedTime / duration);
            _tpsPOV.m_HorizontalAxis.Value = startAngle + newAngle;
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
        // 角度を設定
        _tpsPOV.m_HorizontalAxis.Value = targetAngle;
    }

}

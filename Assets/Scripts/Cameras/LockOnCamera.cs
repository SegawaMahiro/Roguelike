using UnityEngine;
using R3;
using Cinemachine;
using Roguelike.Players;
using Cysharp.Threading.Tasks;

public class LockOnCamera : MonoBehaviour
{
    [SerializeField] Transform _player;

    [SerializeField] Vector3 _defaultDistance;

    [SerializeField] CinemachineVirtualCamera _tpsCamera;
    [SerializeField] CinemachineVirtualCamera _lockonCamera;

    [SerializeField] Transform _lockOnCenter;
    [SerializeField] GameObject _lockonTargetArrow;

    [SerializeField] Transform _targetPointer;
   // [SerializeField] CinemachineTargetGroup _targetGroup;

    // lockonのtarget
    private Transform _target;
    // 現在のカメラ
    private CinemachineVirtualCamera _currentCamera;
    // カメラの視点
    private CinemachinePOV _tpsPOV;
    private CinemachinePOV _lockonPOV;
    private bool _hasTarget;

    private PlayerMove _playerMove;

    private void Awake() {
        Observable.EveryUpdate().Subscribe(_ => UpdateCamera()).AddTo(this);

        _playerMove = _player.GetComponent<PlayerMove>();
        _tpsPOV = _tpsCamera.GetCinemachineComponent<CinemachinePOV>();
        _lockonPOV = _lockonCamera.GetCinemachineComponent<CinemachinePOV>();
        _currentCamera = _tpsCamera;

        Observable.EveryValueChanged(_playerMove.IsLockOn, x => x.CurrentValue)
            .Subscribe(x => {

                if (_target != null) {
                    _hasTarget = IsTargetActive();
                    // targetが存在する場合lockon
                    if (_hasTarget) {
                        OnLockOn(x);
                        return;
                    }
                    // targetが存在しないかつlockon状態の場合プレイヤーの方向を向く
                }
                float targetAngle = _player.eulerAngles.y;
                float currentAngle = _tpsPOV.m_HorizontalAxis.Value;
                RotateToTargetAngleAsync(currentAngle, targetAngle, 0.1f).Forget();

            }).AddTo(this);

        Observable.EveryValueChanged(_playerMove.Target, x => x.CurrentValue)
            .Subscribe(x => _target = x).AddTo(this);
    }
    private void OnLockOn(bool toggle) {
        if (toggle) { ToggleLockOn(); }
        else { ToggleTPSCamera(); }

        _lockonTargetArrow.transform.parent = _target;
        _lockonTargetArrow.transform.position = _target.position + new Vector3(0, 1.5f, 0);

        _lockonTargetArrow.SetActive(toggle);
    }


    private void UpdateCamera() {
        if (!IsTargetActive() && _hasTarget && _currentCamera == _lockonCamera) {
            ToggleTPSCamera();
        }
        if (_target == null) return;
        // playerとtargetの中心座標を取得
        _targetPointer.position = _target.position;
        Vector3 center = (_player.position + _target.position) / 2;
        _lockOnCenter.transform.position = center;
    }
    private bool IsTargetActive() {
        if(_target == null ) return false;
        return _target.gameObject.activeSelf;
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
    private void ToggleTPSCamera() {
        _tpsPOV.m_HorizontalAxis.Value = _lockonPOV.m_HorizontalAxis.Value;
        _tpsPOV.m_VerticalAxis.Value = _lockonPOV.m_VerticalAxis.Value;

        _tpsCamera.enabled = true;
        _lockonCamera.enabled = false;
        _currentCamera = _tpsCamera;
    }

    private void ToggleLockOn() {
        _lockonPOV.m_HorizontalAxis.Value = _tpsPOV.m_HorizontalAxis.Value;
        _lockonPOV.m_VerticalAxis.Value = _tpsPOV.m_VerticalAxis.Value;

        _tpsCamera.enabled = false;
        _lockonCamera.enabled = true;
        _currentCamera = _lockonCamera;
    }
}

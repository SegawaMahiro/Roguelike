using R3;
using UnityEngine;

namespace Roguelike.Players
{
    public class PlayerMove : BasePlayerComponent
    {
        [SerializeField] SerializableReactiveProperty<bool> _isLockOn;
        [SerializeField] AnimationClip _clip;

        private PlayerCharacterController _cc;
        private PlayerAnimation _playerAnimation;

        private float _currentWalkSpeed;

        private ReactiveProperty<float> _walkSpeed = new();
        private ReactiveProperty<bool> _isSprint = new();
        private ReactiveProperty<Vector2> _inputDirection = new();
        private ReactiveProperty<bool> _isDodge = new();
        private ReactiveProperty<Transform> _target = new();

        internal PlayerCharacterController CC { get { return _cc; } }

        internal ReadOnlyReactiveProperty<float> WalkSpeed { get { return _walkSpeed; } }
        internal ReadOnlyReactiveProperty<bool> IsSprint { get { return _isSprint; } }
        internal ReadOnlyReactiveProperty<Vector2> InputDirection { get { return _inputDirection; } }
        internal ReadOnlyReactiveProperty<bool> IsLockOn { get { return _isLockOn; } }
        internal ReadOnlyReactiveProperty<bool> IsDodge { get { return _isDodge; } }
        internal ReadOnlyReactiveProperty<Transform> Target {  get { return _target; } }

        protected override void OnAwake() {
            // このtransformに対して実行させるcharacterの移動
            _cc = new PlayerCharacterController(transform);
        }
        protected override void OnStart() {
            Cursor.lockState = CursorLockMode.Locked;
            TryGetComponent(out _playerAnimation);

            // playerの移動を実行する
            Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Subscribe(_ => {
                    _walkSpeed.Value = Mathf.Lerp(_walkSpeed.Value, _currentWalkSpeed, 3 * Time.deltaTime);
                    _cc.CharacterMove();

                }).AddTo(this);


            // 移動が行われている場合進行方向を取得
            InputProvider.MoveDirection
                .Subscribe(x => {
                    OnMove(x);
                    var inverseDir = transform.InverseTransformDirection(x);
                    _inputDirection.Value = new Vector2(inverseDir.x, inverseDir.z);
                    }
                ).AddTo(this);


            // sprintが有効の場合valueに適応
            InputProvider.Sprint
                .Where(_ => Core.Parameter.IsSprintable && !_isLockOn.Value)
                .Subscribe(x => _isSprint.Value = x).AddTo(this);

            // ロックオン状態を取得
            InputProvider.LockOn
                .Subscribe(x => OnLockOn(x)).AddTo(this);

            // 回避
            InputProvider.Dodge
                .Subscribe(x => OnDodge(x)).AddTo(this);
        }

        private void OnMove(Vector3 direction) {
            if(direction == Vector3.zero || !Core.Parameter.IsMovable) {
                _currentWalkSpeed = 0;
                return;
            }
            _currentWalkSpeed = _isSprint.Value ? Core.Parameter.SprintSpeed : Core.Parameter.WalkSpeed; 
            _currentWalkSpeed *= direction.magnitude;

            // playerの入力と速度に応じて移動を決定させる
            _cc.OnMoveInput(direction, _walkSpeed.Value);

            _inputDirection.Value = direction;
        }
        private void OnLockOn(bool toggle) {
            if (toggle) {
                _isSprint.Value = false;
            }
            _isLockOn.Value = toggle;
            _cc.ToggleLockOn(_isLockOn.Value);
            _cc.SetLockonTarget(_target.Value);
            // targetが特定の範囲内にいた場合
            SetTarget();
        }
        private void OnDodge(bool toggle) {
            _isDodge.Value = toggle;
            // ロックオン中の場合は回避
            if (_isLockOn.Value) {
                Core.DisableMovable(1f);
                _playerAnimation.PlayAnimation(_clip);
                var dir = new Vector3(_inputDirection.Value.x, 0, _inputDirection.Value.y);
                _cc.ApplyKnockback(-transform.forward, 5,1f,0.4f);
                return;
            }
        }
        private void SetTarget() {
            _target.Value = GameObject.Find("Dummy")?.transform;
        }
    }
}

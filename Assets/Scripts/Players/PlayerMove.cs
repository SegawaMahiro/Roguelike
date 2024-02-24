using R3;
using UnityEngine;

namespace Roguelike.Players
{
    public class PlayerMove : BasePlayerComponent
    {
        [SerializeField] Transform _target;
        [SerializeField] SerializableReactiveProperty<bool> _isLockOn;

        private PlayerCharacterController _cc;
        private PlayerAnimation _playerAnimation;

        private float _currentWalkSpeed;

        private ReactiveProperty<float> _walkSpeed = new();
        private ReactiveProperty<bool> _isSprint = new();
        private ReactiveProperty<Vector3> _inputDirection = new();
        private ReactiveProperty<bool> _isDodge = new();

        internal PlayerCharacterController CC { get { return _cc; } }
        public Transform Target { get { return _target; } }

        internal ReadOnlyReactiveProperty<float> WalkSpeed { get { return _walkSpeed; } }
        internal ReadOnlyReactiveProperty<bool> IsSprint { get { return _isSprint; } }
        internal ReadOnlyReactiveProperty<Vector3> InputDirection { get { return _inputDirection; } }
        internal ReadOnlyReactiveProperty<bool> IsLockOn { get { return _isLockOn; } }
        internal ReadOnlyReactiveProperty<bool> IsDodge { get { return _isDodge; } }

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
                .Subscribe(x => OnMove(x)).AddTo(this);


            // sprintが有効の場合valueに適応
            InputProvider.Sprint
                .Where(_ => Core.Parameter.IsSprintable)
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
            _isLockOn.Value = toggle;
            _cc.ToggleLockOn(_isLockOn.Value);
            _cc.SetLockonTarget(_target);
            // targetが特定の範囲内にいた場合
        }
        private void OnDodge(bool toggle) {
            _isDodge.Value = toggle;
            // ロックオン中の場合は回避
            if (_isLockOn.Value) {
                _playerAnimation.PlayAnimation("Dodge", 1);
                _cc.ApplyKnockback(Quaternion.LookRotation(transform.forward) * InputDirection.CurrentValue, 5, 0.3f);
            }
            // それ以外の場合ジャンプ
        }
    }
}

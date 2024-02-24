using Cysharp.Threading.Tasks;
using R3;
using Roguelike.Weapons;
using System;
using UnityEngine;

namespace Roguelike.Players
{
    public class PlayerCombat : BasePlayerComponent
    {
        [SerializeField] MeleeWeapon _holdingWeapon;
        [SerializeField] Transform _playerHand;

        private PlayerAnimation _playerAnimation;
        private PlayerMove _playerMove;
        private PlayerCharacterController _cc;
        private GameObject _holdingWeaponObject;
        private float _cooltime = 0;
        private int _comboCount = 0;

        protected override void OnStart() {
            _holdingWeapon = _holdingWeapon.CreateWeapon();
            TryGetComponent(out _playerMove);
            TryGetComponent(out _playerAnimation);
            _cc = _playerMove.CC;

            InputProvider.Attack
                .Where(x => x && _cooltime <= 0)
                .Subscribe(_ => {
                    OnAttack().Forget();
                }).AddTo(this);

            Observable.EveryUpdate().Subscribe(_ => {
                if (_cooltime > 0) {
                    _cooltime -= Time.deltaTime;
                }
            }).AddTo(this);
        }

        /// <summary>
        /// 攻撃時に行う処理
        /// </summary>
        private async UniTask OnAttack() {
            // 武器をもっていない場合現在設定中の武器を生成する
            if (_holdingWeaponObject is null) {
                _holdingWeaponObject = _holdingWeapon.HoldWeapon(_playerHand);
            }
            // 当たり判定を作成
            _holdingWeapon.CreateHitbox(gameObject, _comboCount);

            // 再生するainmationの時間を取得
            var time = _holdingWeapon.GetAttackTime(_comboCount);
            var clipTiem = time.dura + time.delay;

          // 移動を無効化し、animationに応じて移動を行う
            Core.DisableMovable(clipTiem);
            PlayAttackAnimation();
            ApplyAnimationMoveAsync(clipTiem).Forget();
            SetCooltime(clipTiem);

　　　      // animation終了時次のコンボを設定
            await UniTask.Delay(TimeSpan.FromSeconds(time.delay), cancellationToken: destroyCancellationToken);
            _comboCount++;
            if (_comboCount >= _holdingWeapon.Combo.Count) {
                _comboCount = 0;
            }
        }

        /// <summary>
        /// animationを再生する
        /// </summary>
        private void PlayAttackAnimation() {
            var attackAnim = _holdingWeapon.GetAttackAnimation(_comboCount);
            _playerAnimation.PlayAnimation(attackAnim);
        }

        private void SetCooltime(float ct) {
            _cooltime = ct;
        }
        /// <summary>
        /// animationに対応したplayerの移動
        /// </summary>
        private async UniTask ApplyAnimationMoveAsync(float clipTime) {
            // 移動が指定されていない場合行わない
            if (!_holdingWeapon.IsMovableAnimation(_comboCount)) {
                return;
            }

            // 待機時間
            var moveParam = _holdingWeapon.GetMove(_comboCount);
            await UniTask.Delay(TimeSpan.FromSeconds(moveParam.MoveTiming * clipTime), cancellationToken: destroyCancellationToken);

            // 移動方向
            var direction = GetMoveDirection(moveParam.Direction);
            float power = moveParam.Power;

            // lockon状態の場合は対象へ向かっての移動
            if (_playerMove.IsLockOn.CurrentValue) {
                Vector3 closestPoint = _playerMove.Target.parent.GetComponent<Collider>().ClosestPoint(transform.position);
                power = (Vector3.Distance(closestPoint, transform.position)) / moveParam.Duration;
                power = Mathf.Clamp(power, 0, 20);
            }

            _cc.ApplyKnockback(direction, power, moveParam.Duration);
        }

        private Vector3 GetMoveDirection(Weapons.Combo.MoveSettings.MoveDirection direction) {
            return direction switch {
                Weapons.Combo.MoveSettings.MoveDirection.Forward => transform.forward,
                Weapons.Combo.MoveSettings.MoveDirection.Backward => -transform.forward,
                Weapons.Combo.MoveSettings.MoveDirection.Left => -transform.right,
                Weapons.Combo.MoveSettings.MoveDirection.Right => transform.right,
                _ => throw new InvalidOperationException()
            };
        }
    }
}

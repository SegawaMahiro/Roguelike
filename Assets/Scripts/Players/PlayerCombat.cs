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
        [SerializeField, Range(0, 1)] float _nextActionAcceptTime;
        [SerializeField] float _assistMaxDistance = 3;
        [SerializeField, Range(0, 1)] float _assistSpeed = 0.2f;
        [SerializeField, Range(0, 1)] float _assistPower = 0.5f;

        private Transform _target;
        private PlayerAnimation _playerAnimation;
        private PlayerMove _playerMove;
        private PlayerCharacterController _cc;

        private GameObject _holdingWeaponObject;
        private float _cooltime = 0;
        private float _initialCooltime = 0;
        private int _comboCount = 0;
        private bool _hasNextAttack;

        protected override void OnStart() {
            _holdingWeapon = _holdingWeapon.CreateWeapon();
            TryGetComponent(out _playerMove);
            TryGetComponent(out _playerAnimation);
            _cc = _playerMove.CC;

            // 攻撃の入力を受け取り
            InputProvider.Attack
                .Where(_ => !_hasNextAttack)
                .Subscribe(_ => {
                    OnAttack();
                }).AddTo(this);

            Observable.EveryUpdate().Subscribe(_ => {
                // cooltimeを減少し続ける
                if (_cooltime > 0) {
                    _cooltime -= Time.deltaTime;
                }
                // cooltime終了時次の攻撃が入力されている場合攻撃
                if (_cooltime < 0 && _hasNextAttack) {
                    Attack();
                }
            }).AddTo(this);

            Observable.EveryValueChanged(_playerMove.Target, x => x.CurrentValue)
                .Subscribe(x => _target = x).AddTo(this);
            Observable.EveryValueChanged(_playerMove.IsDodge, x => x.CurrentValue)
                .Subscribe(_ => CancelAttack()).AddTo(this);
        }

        /// <summary>
        /// 攻撃時に行う処理
        /// </summary>
        private void OnAttack() {
            // 移動できない場合攻撃も不可能
            if (_cooltime <= 0 && !_hasNextAttack && Core.Parameter.IsMovable) {
                Attack();
                return;
            }
            // 次の攻撃が受付可能な割合を超えていた場合次の攻撃を保存
            if (_cooltime < _initialCooltime * (1 - _nextActionAcceptTime) && !_hasNextAttack) {
                _hasNextAttack = true;
            }
        }

        private void Attack() {
            if (!Core.Parameter.IsMovable) return;
            // 武器をもっていない場合現在設定中の武器を生成する
            if (_holdingWeaponObject is null) {
                _holdingWeaponObject = _holdingWeapon.HoldWeapon(_playerHand);
            }
            // 当たり判定を作成
            _holdingWeapon.CreateHitbox(gameObject, _comboCount);

            var currentCombo = _holdingWeapon.GetCombo(_comboCount);
            // 再生するainmationの時間を取得
            var clipLength = currentCombo.ClipLength;

            // 移動を無効化し、animationに応じて移動を行う
            Core.DisableMovable(clipLength - currentCombo.StartTime);
            SetCooltime(clipLength - currentCombo.StartTime);
            _playerAnimation.PlayAnimation(currentCombo.AnimationClip);
            ApplyAnimationMoveAsync(currentCombo).Forget();

            _comboCount++;
            if (_comboCount >= _holdingWeapon.Combo.Count) {
                _comboCount = 0;
            }
        }

        private void SetCooltime(float ct) {
            _cooltime = ct;
            _initialCooltime = ct;
            _hasNextAttack = false;
        }
        /// <summary>
        /// animationに対応したplayerの移動
        /// </summary>
        private async UniTask ApplyAnimationMoveAsync(ComboParameter combo) {
            if (!_holdingWeapon) return;

            // 移動用のパラメータ取得
            var moveParam = combo.MoveSetting;
            // 移動開始までを待機
            var startTime = TimeSpan.FromSeconds(moveParam.StartTime * combo.ClipLength);
            await UniTask.Delay(startTime, cancellationToken: destroyCancellationToken);

            // 移動方向
            var direction = GetMoveDirection(moveParam.Direction);
            float duration = moveParam.Duration;
            float power = moveParam.Power;

            // ロックオン中
            if (_target != null && _playerMove.IsLockOn.CurrentValue) {
                // 距離が一定の範囲内の場合攻撃をアシストする
                var targetDistance = Vector3.Distance(_target.position, _holdingWeapon.WeaponCenter.position);
                if (targetDistance < 3f) return;
                if (_assistMaxDistance > targetDistance) {
                    // 移動距離の上書き
                    power = targetDistance;
                    direction = (_target.position - transform.position).normalized;
                    direction.y = 0;
                    duration = _assistSpeed;
                }
            }

            _cc.ApplyKnockback(direction, power, duration);
        }


        /// <summary>
        /// 角度を取得
        /// </summary>
        private Vector3 GetMoveDirection(MoveSettings.MoveDirection direction) {
            return direction switch {
                MoveSettings.MoveDirection.Forward => transform.forward,
                MoveSettings.MoveDirection.Backward => -transform.forward,
                MoveSettings.MoveDirection.Left => -transform.right,
                MoveSettings.MoveDirection.Right => transform.right,
                _ => throw new InvalidOperationException()
            };
        }
        private void CancelAttack() {
            _holdingWeapon.CancelAttack();
            _hasNextAttack = false;
        }
    }
}

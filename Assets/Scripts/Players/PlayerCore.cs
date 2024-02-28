using R3;
using Roguelike.Damages;
using UnityEngine;

namespace Roguelike.Players
{
    public class PlayerCore : MonoBehaviour, IDamageApplicable
    {
        [SerializeField] PlayerParameter _parameter;
        private IPlayerState _playerState;
        internal PlayerParameter Parameter { get { return _parameter; } }
        internal IPlayerState PlayerState { get { return _playerState; } }

        private float _moveDisableTimer;


        private bool _isInvincible;
        private float _invincibleTime;

        private void Awake() {
            _parameter.CurrentHealth = _parameter.MaxHealth;

            Observable.EveryUpdate().Subscribe(_ => {
                if (_invincibleTime > 0) {
                    _invincibleTime -= Time.deltaTime;
                }

                if(_moveDisableTimer > 0) {
                    _moveDisableTimer -= Time.deltaTime;
                }
                else {
                    _parameter.IsMovable = true;
                }
            }).AddTo(this);
        }
        public void ApplyDamage(Damage damage) {
            if (_isInvincible) return;

            _invincibleTime = _parameter.InvincibleTime;
            _parameter.CurrentHealth -= damage.Value;
            if(_parameter.CurrentHealth <= 0) {
                Debug.Log("Dead");
                // 死亡時処理
            }
        }
        public void DisableMovable(float duration) {
            _parameter.IsMovable = false;
            _moveDisableTimer = duration;
        }
    }
}
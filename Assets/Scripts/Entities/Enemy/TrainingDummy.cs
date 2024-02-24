using UnityEngine;
using UnityEngine.UI;
using Roguelike.Damages;

namespace Roguelike.Entities.Enemy
{
    internal class TrainingDummy : MonoBehaviour, IDamageApplicable
    {
        [SerializeField] int _health;
        [SerializeField] int _maxhealth;

        [SerializeField] AnimationClip _dmg;
        [SerializeField] AnimationClip _dead;

        [SerializeField] Slider _healthSlider;

        [SerializeField] GameObject _deadPref;
        [SerializeField] float _moveSpeed = 10;

        [SerializeField] Transform _playerTransform; // プレイヤーのTransform
        [SerializeField] float _followDistance = 5f; // プレイヤーを追尾する距離

        Animation _anim;
        Rigidbody _rb;

        bool _movingRight;

        private void Awake() {
            _anim = GetComponent<Animation>();
            TryGetComponent(out _rb);
            _playerTransform = GameObject.Find("Player").transform;
        }
        private void Update() {
            // プレイヤーとの距離を計算
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer > _followDistance) {
                // プレイヤーの方向を向く
                Vector3 lookDirection = (_playerTransform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * _moveSpeed);

                // プレイヤーに向かって力を加える
                _rb.AddForce(transform.forward * _moveSpeed, ForceMode.Force);
            }
            else {
                // 左右に移動
                if (_movingRight) {
                    _rb.AddForce(Vector3.right * _moveSpeed, ForceMode.Force);
                }
                else {
                    _rb.AddForce(Vector3.left * _moveSpeed, ForceMode.Force);
                }

                // 移動距離を超えたら方向を切り替える
                if (Mathf.Abs(transform.position.x - 3) > 3) {
                    _movingRight = !_movingRight;
                }
            }
        }


        public void ApplyDamage(Damage damage) {
            _health -= 15;
            _healthSlider.value = (float)_health / _maxhealth;
            _rb.AddForce(-transform.forward * 50, ForceMode.Impulse);
            if (_health > 0) {
                _anim.clip = _dmg;
                _anim.Play();
            }
            else {
                _anim.clip = _dead;
                GameObject p = ObjectPoolManager.Instance.Get(_deadPref, transform.position, Quaternion.identity);
                ObjectPoolManager.Instance.Release(p, 2);
                _anim.Play();
                _health = _maxhealth;
                _healthSlider.value = (float)_health / _maxhealth;
            }
        }
    }
}

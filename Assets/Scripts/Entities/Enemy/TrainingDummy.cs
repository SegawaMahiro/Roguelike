using UnityEngine;
using UnityEngine.UI;
using Roguelike.Damages;

namespace Roguelike.Entities.Enemy
{
    internal class TrainingDummy : MonoBehaviour, IDamageApplicable
    {
        [SerializeField] int _health;
        [SerializeField] int _maxhealth;

        [SerializeField] Slider _healthSlider;

        public void ApplyDamage(Damage damage) {
            _health -= 15;
            _healthSlider.value = (float)_health / _maxhealth;
            if (_health > 0) {
            }
            else {
                Destroy(gameObject);
                _healthSlider.value = (float)_health / _maxhealth;
            }
        }
    }
}

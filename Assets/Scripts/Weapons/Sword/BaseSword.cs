using Roguelike.Damages;
using Roguelike.Damages.Hitbox;
using UnityEngine;

namespace Roguelike.Weapons.Sword
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Weapons/Create Sword", fileName = "SwordData")]
    public class BaseSword : MeleeWeapon
    {
        [SerializeField] GameObject _hitVFX;
        [SerializeReference,SubclassSelector] IHitboxShape _hitShape;

        protected override void OnAttack(int comboCount) {

            var t = GetCombo(comboCount);

            HitboxUtility.CreateContinuousOverlap(_weaponCenter, _hitShape, t.StartTime, t.Duration, hit => {

                if (hit.Collider.gameObject == _weaponOwner) return;

                if (hit.Collider.TryGetComponent<IDamageApplicable>(out var target)) {

                    var center = _weaponCenter.position;
                    var enter = hit.EnterPosition;
                    var hitDirection = hit.EnterDirection;

                    var rotation = Quaternion.LookRotation(hitDirection);
                    var vfx = ObjectPoolManager.Instance.Get(_hitVFX, enter, rotation);
                    ObjectPoolManager.Instance.Release(vfx, 1);
                    SoundManager.Instance.PlaySE($"SwordHit{comboCount+1}");


                    target.ApplyDamage(new Damage() { Value = _strength });

                    _source.Cancel();
                }

            }, _source.Token);
        }
    }
}

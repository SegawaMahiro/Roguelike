using UnityEngine;

namespace Roguelike.Entities.InteractiveObjects
{
    internal class PotObject : BaseInteractiveObject
    {
        [SerializeField] GameObject _goldVFX;
        protected override void OnDamage() {
            
            var vfx = ObjectPoolManager.Instance.Get(_goldVFX, transform.position, Quaternion.identity);
            ObjectPoolManager.Instance.Release(vfx, 1);
            Destroy(gameObject);
        }
    }
}

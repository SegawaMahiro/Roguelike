using Roguelike.Damages;
using UnityEngine;

namespace Roguelike.Entities.InteractiveObjects
{
    internal abstract class BaseInteractiveObject : MonoBehaviour, IDamageApplicable{
        public void ApplyDamage(Damage damage) {
            OnDamage();
        }
        protected abstract void OnDamage();
    }
}

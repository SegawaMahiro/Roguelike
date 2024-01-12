using Assets.Scripts.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapons.Sword
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Weapons/Create Sword", fileName = "SwordData")]
    public class BaseSword : Weapon
    {
        [SerializeReference,SubclassSelector] BaseElement _weaponElement;
        [SerializeField] uint _damage;
        protected override BaseElement WeaponElement => _weaponElement;
        protected override uint Strength => _damage;

        public override void Attack(uint combo) {
            Debug.Log("Damage : " + _damage);
            Debug.Log("elementDamage : " +_weaponElement.ElementDamage);
        }
    }
}

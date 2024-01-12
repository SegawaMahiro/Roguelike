using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Elements;

namespace Assets.Scripts.Weapons
{
    public abstract class Weapon : ScriptableObject
    {
        protected abstract BaseElement WeaponElement { get; }
        protected abstract uint Strength { get; }

        public abstract void Attack(uint combo);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roguelike.Elements
{
    [Serializable]
    public class FireElement : BaseElement
    {
        [SerializeField] int _elementDamage;
        public override int ElementDamage { get { return _elementDamage; } }
    }
}

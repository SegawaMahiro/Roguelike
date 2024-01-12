using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Elements
{
    public class FireElement : BaseElement
    {
        [SerializeField] int _elementDamage;
        public override int ElementDamage { get { return _elementDamage; } }
    }
}

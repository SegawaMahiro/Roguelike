using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Inputs
{
    internal interface IInputProvider
    {
        internal Vector2 PlayerMove {  get; }
        internal Vector2 CameraMove { get; }
        internal bool Attack {  get; }

    }
}

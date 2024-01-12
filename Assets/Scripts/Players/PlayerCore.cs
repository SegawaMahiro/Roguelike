using System.Collections;
using UnityEngine;
using Assets.Scripts.Damages;

namespace Assets.Scripts.Players
{
    public class PlayerCore : MonoBehaviour, IDamageApplicable
    {
        [SerializeField] PlayerParameter _defaultParameters;
        public PlayerParameter DefaultPlayerParameters { get { return _defaultParameters; } }

        public void ApplyDamage(Damage damage) {

        }
    }
}
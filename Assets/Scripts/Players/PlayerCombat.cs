using System.Collections;
using UnityEngine;
using UniRx;
using Assets.Scripts.Weapons;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Players
{
    public class PlayerCombat : PlayerBaseComponent
    {
        [SerializeField] Weapon _holdingWeapon;
        protected override void OnStart() {
            InputProvider.Player.Fire.ObserveStarted()
                .Subscribe(x => {
                    OnAttack();
                }).AddTo(this);
        }
        private void OnAttack() {
            _holdingWeapon.Attack(1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assets.Scripts.Utils;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Players
{
    public class PlayerMove : PlayerBaseComponent
    {
        private Rigidbody _rb;
        
        protected override void OnStart() {
            _rb = GetComponent<Rigidbody>();

            // 移動
            InputProvider.Player.Move.ObserveEveryPressing()
                .Select(x => x.action.ReadValue<Vector2>().normalized)
                .Where(x => x.magnitude > 0)
                .Subscribe(x => {
                    OnMove(x);
                }).AddTo(this);
        }
        private void OnMove(Vector2 direction) {
            var velocity = new Vector3(direction.x, 0, direction.y) * Parameter.WalkSpeed;
            velocity.y = _rb.velocity.y;
            _rb.velocity = velocity;
        }
    }
}

using System.Collections;
using UnityEngine;
using Assets.Inputs;

namespace Assets.Scripts.Players
{
    [RequireComponent(typeof(PlayerCore))]
    public abstract class PlayerBaseComponent : MonoBehaviour
    {
        private MainInputAction _inputProvider;
        private PlayerCore _playerCore;
        protected PlayerCore Core { get { return _playerCore; } } 
        protected MainInputAction InputProvider { get { return _inputProvider; } }
        protected PlayerParameter Parameter { get { return Core.DefaultPlayerParameters; } }

        void Start() {
            _playerCore = GetComponent<PlayerCore>();
            _inputProvider = new MainInputAction();
            _inputProvider.Player.Enable();
            OnStart();
        }

        private void OnDestroy() {
            _inputProvider.Dispose();
        }
        protected abstract void OnStart();
    }
}
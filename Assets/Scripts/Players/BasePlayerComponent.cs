using UnityEngine;

namespace Roguelike.Players
{
    [RequireComponent(typeof(PlayerCore))]
    public abstract class BasePlayerComponent : MonoBehaviour
    {
        private PlayerCore _playerCore;
        private PlayerInput _input;

        protected PlayerCore Core { get { return _playerCore; } } 
        protected PlayerInput InputProvider { get { return _input; } }

        private void Awake() {
            OnAwake();
        }
        void Start() {
            TryGetComponent(out _playerCore);
            TryGetComponent(out _input);
            OnStart();
        }
        protected abstract void OnStart();
        protected virtual void OnAwake() { }
    }
}
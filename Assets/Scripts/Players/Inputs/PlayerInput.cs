using Assets.Inputs;
using R3;
using Roguelike.Inputs;
using UnityEngine;

namespace Roguelike.Players
{
    public class PlayerInput : BasePlayerComponent
    {
        private MainInputAction _inputProvider ;

        [SerializeField] SerializableReactiveProperty<Vector3> _moveDirection = new();
        [SerializeField] SerializableReactiveProperty<bool> _sprint = new();
        [SerializeField] SerializableReactiveProperty<bool> _lockOn = new();
        [SerializeField] SerializableReactiveProperty<bool> _dodge = new();
        [SerializeField] SerializableReactiveProperty<bool> _attack = new();

        internal ReadOnlyReactiveProperty<Vector3> MoveDirection { get { return _moveDirection; } }
        internal ReadOnlyReactiveProperty<bool> Sprint { get { return _sprint; } }
        internal ReadOnlyReactiveProperty<bool> LockOn { get { return _lockOn; } }
        internal ReadOnlyReactiveProperty<bool> Dodge {  get { return _dodge; } }
        internal ReadOnlyReactiveProperty<bool> Attack { get { return _attack; } }
        protected override void OnStart() {
        }
        protected override void OnAwake() {
            _inputProvider = new();
            _inputProvider.Enable();
            _inputProvider.AddTo(this);

            var moveDirection = _inputProvider.Player.Move;

            // 移動
            _inputProvider.Player.Move.ObserveEveryPressing()
                .Select(x => x.action.ReadValue<Vector2>())
                .Where(x => x.magnitude > 0)
                .Subscribe(x => _moveDirection.OnNext(new Vector3(x.x, 0, x.y))).AddTo(this);

            _inputProvider.Player.Move.ObserveCanceled()
                .Subscribe(x => _moveDirection.OnNext(Vector3.zero)).AddTo(this);

            // ダッシュ
            _inputProvider.Player.Sprint.ObserveStartAndEnd()
                .Subscribe(_ => _sprint.Value = !_sprint.Value).AddTo(this);

            // ロックオン
            _inputProvider.Player.LockOn.ObserveStartAndEnd()
                .Subscribe(_ => _lockOn.Value = !_lockOn.Value).AddTo(this);

            // 回避
            _inputProvider.Player.Dodge.ObserveStartAndEnd()
                .Subscribe(_ => _dodge.Value = !_dodge.Value).AddTo(this);

            // 攻撃
            _inputProvider.Player.Attack.ObserveStartAndEnd()
                .Subscribe(_ => _attack.Value = !_attack.Value).AddTo(this);
        }
    }
}

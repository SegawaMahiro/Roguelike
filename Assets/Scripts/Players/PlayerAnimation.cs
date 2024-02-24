using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Roguelike.Players
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : BasePlayerComponent, IMatchTarget
    {
        [SerializeField] AnimatorOverrideController _lockonOverride;
        private RuntimeAnimatorController _defaultController;

        private Animator _animator;
        private PlayerMove _playerMove;

        private string _currentStateName;
        private float _transitionTime = 0f;
        private float _idleTransitionTime = 0.3f;

        private const string DefaultState = "Idle";
        public Animator Animator { get { return _animator; } }
        private float WalkSpeed {
            set { _animator.SetFloat("WalkSpeed", value); }
        }
        private Vector3 Direction {
            set {
                _animator.SetFloat("DirectionX", value.x);
                _animator.SetFloat("DirectionZ", value.z);
            }
        }

        public Vector3 TargetPosition => throw new System.NotImplementedException();

        protected override void OnStart() {
            TryGetComponent(out _animator);
            TryGetComponent(out _playerMove);

            _defaultController = _animator.runtimeAnimatorController;

            _playerMove.InputDirection.Subscribe(x =>  Direction = x);
            _playerMove.WalkSpeed.Subscribe(x => WalkSpeed = x);
            _playerMove.IsLockOn
                .Subscribe(x => {
                    if (x) {
                        _animator.runtimeAnimatorController = _lockonOverride;
                    }
                    else {
                        _animator.runtimeAnimatorController = _defaultController;
                    }
                });

            Observable.EveryUpdate().Subscribe(_ => {
                if(_transitionTime >= 0) {
                    _transitionTime -= Time.deltaTime;
                }
                else {
                    if (_currentStateName != DefaultState) {
                        PlayAnimation(DefaultState, _idleTransitionTime);
                    }
                }
            }).AddTo(this);
        }

        internal void PlayAnimation(AnimationClip clip, float blendTime = 0.5f) {
            if (clip.name == _currentStateName) return;
            _animator.CrossFadeInFixedTime(clip.name, blendTime);
            _transitionTime = clip.length * blendTime;
            _currentStateName = clip.name;
        }
        internal void PlayAnimation(string name,float duration,float blendTime = 0.5f) {
            if (name == _currentStateName) return;
            _animator.CrossFadeInFixedTime(name, blendTime);
            _transitionTime = duration * blendTime;
            _currentStateName = name;
        }
    }
}

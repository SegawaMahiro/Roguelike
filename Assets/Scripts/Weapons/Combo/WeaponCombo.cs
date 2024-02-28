using System;
using UnityEngine;

namespace Roguelike.Weapons
{

    [Serializable]
    public struct MoveSettings
    {
        [SerializeField,MinMaxSlider(0, 1)] Vector2 _moveTiming;
        [SerializeField] MoveDirection _direction;
        [SerializeField] float _power;

        public MoveDirection Direction => _direction;
        public float Power => _power;

        public float StartTime => _moveTiming.x;
        public float EndTime => _moveTiming.y;
        public float Duration => _moveTiming.y - _moveTiming.x;

        public enum MoveDirection
        {
            Forward,
            Backward,
            Left,
            Right
        }
    }
    [Serializable]
    public class ComboParameter
    {
        [SerializeField] AnimationClip _animationClip;
        [SerializeField, MinMaxSlider(0, 1)] Vector2 _activationTime;
        [SerializeField] bool _useAnimationMove;
        [SerializeField] MoveSettings _moveSetting;
        public bool IsMovableAnimation => _useAnimationMove;
        public AnimationClip AnimationClip => _animationClip;
        public Vector2 ActivationTime => _activationTime;
        public MoveSettings MoveSetting => _moveSetting;

        public float ClipLength => _animationClip.length;
        public float StartTime => _activationTime.x * _animationClip.length; 
        public float EndTime => _activationTime.y * _animationClip.length;
        public float Duration => EndTime - StartTime;

        public float NormalizedStartTime => _activationTime.x; 
        public float NormalizedEndTime => _activationTime.y;
        public float NormalizedDuration => _activationTime.y - _activationTime.x;
    }

    [CreateAssetMenu(fileName = "ScriptableObjects/Combo/Create ComboParameter", menuName = "New ComboParameter")]
    public class WeaponCombo : ScriptableObject
    {
        [SerializeField] ComboParameter[] _comboParameters;
        public int Count { get { return _comboParameters.Length; } }
        public ComboParameter GetData(int index) => _comboParameters[index];
    }
}
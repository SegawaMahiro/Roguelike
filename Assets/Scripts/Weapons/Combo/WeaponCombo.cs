using System;
using UnityEngine;

namespace Roguelike.Weapons.Combo
{

    [Serializable]
    public struct MoveSettings
    {
        [Range(0, 1)] public float MoveTiming;
        public MoveDirection Direction;
        public float Duration;
        public float Power;

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
        public bool UseAnimationMove => _useAnimationMove;
        public AnimationClip AnimationClip => _animationClip;
        public Vector2 ActivationTime => _activationTime;
        public MoveSettings MoveSetting => _moveSetting;
    }

    [CreateAssetMenu(fileName = "ScriptableObjects/Combo/Create ComboParameter", menuName = "New ComboParameter")]
    public class WeaponCombo : ScriptableObject
    {

        [SerializeField] ComboParameter[] _comboParameters;
        public int Count { get { return _comboParameters.Length; } }
        public MoveSettings GetMoveSettings(int index) => _comboParameters[index].MoveSetting;
        public AnimationClip GetComboAnimation(int index) => _comboParameters[index].AnimationClip;
        public bool GetMoveAnimationToggle(int index) => _comboParameters[index].UseAnimationMove;

        public (float delay, float duration) GetHitboxLifetime(int count) {
            var comboParam = _comboParameters[count];


            float animationLength = comboParam.AnimationClip.length;
            float delay = animationLength * comboParam.ActivationTime.x;
            float duration = animationLength * comboParam.ActivationTime.y - delay;
            return (delay, duration);
        }
    }
}
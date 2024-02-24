using UnityEngine;

namespace Roguelike.Players
{
    [System.Serializable]
    public struct PlayerParameter
    {
        [Header("Combat :")]
        public int MaxHealth;
        public int CurrentHealth;

        [Header("Locomotion :")]
        public bool IsMovable;
        public bool IsSprintable;
        public float WalkSpeed;
        public float SprintSpeed;

        [Header("State :")]
        public float InvincibleTime;
    }
}

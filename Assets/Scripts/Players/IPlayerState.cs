namespace Roguelike.Players
{
    internal interface IPlayerState
    {
        public enum PlayerState { 
            Idle,
            Move,
            Attacking,
            Rolling,
            Knockback,
            Dead
        }
        PlayerState State { get; }

        internal bool ChangeState(IPlayerState state);
        internal void OnEnter();

        internal void Execute();

        internal void OnExit();
    }
}

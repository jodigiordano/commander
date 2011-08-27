namespace EphemereGames.Core.Physics
{
    public interface IDestroyable : ICollidable
    {
        bool Alive { get; }
    }
}

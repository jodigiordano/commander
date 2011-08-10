namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;

    public enum Shape
    {
        Circle,
        Rectangle,
        Line
    }

    public interface ICollidable : IPhysical
    {
        Vector3 Direction           { get; set; }

        Shape Shape                 { get; set; }
        Circle Circle               { get; set; }
        PhysicalRectangle Rectangle { get; set; }
        Line Line                   { get; set; }
    }
}
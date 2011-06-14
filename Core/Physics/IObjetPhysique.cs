namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;

    public enum Shape
    {
        Circle,
        Rectangle,
        Line
    }

    public interface IObjetPhysique
    {
        Vector3 Position            { get; set; }
        float Speed                 { get; set; }
        Vector3 Direction           { get; set; }
        float Rotation              { get; set; }

        Shape Shape                 { get; set; }
        Circle Circle               { get; set; }
        RectanglePhysique Rectangle { get; set; }
        Line Line                  { get; set; }
    }
}
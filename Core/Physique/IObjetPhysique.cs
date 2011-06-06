namespace EphemereGames.Core.Physique
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
        Cercle Circle               { get; set; }
        RectanglePhysique Rectangle { get; set; }
        Ligne Line                  { get; set; }
    }
}
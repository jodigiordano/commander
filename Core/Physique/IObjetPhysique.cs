namespace EphemereGames.Core.Physique
{
    using Microsoft.Xna.Framework;

    public enum Forme
    {
        Cercle,
        Rectangle,
        Ligne
    }

    public interface IObjetPhysique
    {
        Vector3 Position            { get; set; }
        float Vitesse               { get; set; }
        Vector3 Direction           { get; set; }
        float Rotation              { get; set; }

        Forme Forme                 { get; set; }
        Cercle Cercle               { get; set; }
        RectanglePhysique Rectangle { get; set; }
        Ligne Ligne                 { get; set; }
    }
}
namespace TDA
{
    using Microsoft.Xna.Framework;
    using RainingSundays.Core.Physique;

    public enum Forme
    {
        Cercle,
        Rectangle,
        Ligne
    }

    interface IObjetPhysique
    {
        Vector3 Position    { get; set; }
        float Vitesse       { get; set; }
        Vector3 Direction   { get; set; }

        Forme Forme         { get; set; }
        Cercle Cercle       { get; set; }
        Rectangle Rectangle { get; set; }
        Ligne Ligne         { get; set; }
    }
}
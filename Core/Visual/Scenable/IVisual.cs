namespace EphemereGames.Core.Visual
{
    using Microsoft.Xna.Framework;


    public interface IVisual
    {
        Rectangle VisiblePart   { set; }
        Vector2 Origin          { get; set; }
        Vector2 Size            { get; set; }
        Color Color             { get; set; }
        byte Alpha              { get; set; }
    }
}

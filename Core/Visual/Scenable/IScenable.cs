namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public enum TypeBlend
    {
        Default,
        Multiply,
        None,
        Add,
        Alpha,
        Substract
    }


    public interface IScenable
    {
        int Id                      { get; }
        Vector3 Position            { get; set; }
        double VisualPriority       { get; set; }
        TypeBlend Blend             { get; set; }

        void Draw(SpriteBatch spriteBatch);
    }
}

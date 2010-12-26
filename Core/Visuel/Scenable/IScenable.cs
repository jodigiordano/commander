namespace EphemereGames.Core.Visuel
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
        Vector3 Position            { get; set; }
        float VisualPriority        { get; set; }
        TypeBlend Blend             { get; set; }
        List<IScenable> Components  { get; set; }

        void Draw(SpriteBatch spriteBatch);
    }
}

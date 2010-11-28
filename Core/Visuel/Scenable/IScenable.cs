using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Visuel
{
    /// <summary>
    /// Mélange (blending) utilisé lors de l'affichage
    /// </summary>
    public enum TypeMelange
    {
        Default,
        Multiplicatif,
        Aucun,
        Additif,
        Alpha,
        Soustraire
    }

    public interface IScenable
    {
        Vector3 Position                { get; set; }
        float PrioriteAffichage         { get; set; }
        Scene Scene                     { set; }
        TypeMelange Melange             { get; set; }
        List<IScenable> Composants      { get; set; }

        void Draw(SpriteBatch spriteBatch);
    }
}

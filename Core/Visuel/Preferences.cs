//=============================================================================
//
// Préférences de la librairie
// Certaines doivent être settées avant d'utiliser la librairie
//
//=============================================================================

namespace EphemereGames.Core.Visuel
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using EphemereGames.Core.Utilities;

    static class Preferences
    {
        public static GraphicsDeviceManager GraphicsDeviceManager  { get; set; }   // Le gestionnaire de la carte graphique
        public static int WindowWidth                           { get; set; }   // La hauteur de la fenêtre
        public static int WindowHeight                           { get; set; }   // La largeur de la fenêtre
        public static ContentManager Content                       { get; set; }   // Le gestionnaire de contenu
        public static float Luminosite                             { get; set; }   // Niveau de luminosité général
        public static float Contraste                              { get; set; }   // Niveau de contraste général
        public static ManagedThread ThreadParticules               { get; set; }   // Thread où sont calculées les particules
        public static ManagedThread ThreadLogic                  { get; set; }   // Thread où est calculé la logique d'une scène
    }
}

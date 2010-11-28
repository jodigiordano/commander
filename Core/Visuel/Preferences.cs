//=============================================================================
//
// Préférences de la librairie
// Certaines doivent être settées avant d'utiliser la librairie
//
//=============================================================================

namespace Core.Visuel
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;

    static class Preferences
    {
        public static GraphicsDeviceManager GraphicsDeviceManager  { get; set; }   // Le gestionnaire de la carte graphique
        public static int FenetreLargeur                           { get; set; }   // La hauteur de la fenêtre
        public static int FenetreHauteur                           { get; set; }   // La largeur de la fenêtre
        public static ContentManager Content                       { get; set; }   // Le gestionnaire de contenu
        public static float Luminosite                             { get; set; }   // Niveau de luminosité général
        public static float Contraste                              { get; set; }   // Niveau de contraste général
        public static ManagedThread ThreadParticules               { get; set; }   // Thread où sont calculées les particules
        public static ManagedThread ThreadLogique                  { get; set; }   // Thread où est calculé la logique d'une scène
    }
}

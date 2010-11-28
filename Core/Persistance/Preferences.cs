//=============================================================================
//
// Préférences de la librairie
// Certaines doivent être settées avant d'utiliser la librairie
//
//=============================================================================

namespace Core.Persistance
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;

    static class Preferences
    {
        public static String DossierContenu                     { get; set; }
        public static String CheminRelatifPackages              { get; set; }
        public static GameServiceContainer GameServiceContainer { get; set; }
        public static ManagedThread ThreadContenu               { get; set; }
        public static ManagedThread ThreadDonnees               { get; set; }
        public static StorageMessages StorageMessages           { get; set; }
    }
}

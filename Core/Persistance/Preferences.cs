//=============================================================================
//
// Préférences de la librairie
// Certaines doivent être settées avant d'utiliser la librairie
//
//=============================================================================

namespace EphemereGames.Core.Persistance
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using EphemereGames.Core.Utilities;

    static class Preferences
    {
        public static String ContentFolderPath                     { get; set; }
        public static String PackagesFolderPath              { get; set; }
        public static GameServiceContainer GameServiceContainer { get; set; }
        public static ManagedThread ThreadContent               { get; set; }
        public static ManagedThread ThreadData               { get; set; }
    }
}

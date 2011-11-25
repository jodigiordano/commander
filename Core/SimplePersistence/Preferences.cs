namespace EphemereGames.Core.SimplePersistence
{
    using Microsoft.Xna.Framework;


    static class Preferences
    {
        public static string ContentFolderPath                      { get; set; }
        public static string PackagesFolderPath                     { get; set; }
        public static GameServiceContainer GameServiceContainer     { get; set; }
    }
}

namespace EphemereGames.Core.Visual
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;


    static class Preferences
    {
        public static Random Random = new Random();

        public static GraphicsDeviceManager GraphicsDeviceManager   { get; set; }
        public static int WindowWidth                               { get; set; }
        public static int WindowHeight                              { get; set; }
        public static ContentManager Content                        { get; set; }
        public static float Brightness                              { get; set; }
        public static float Constrast                               { get; set; }
    }
}

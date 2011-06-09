﻿namespace EphemereGames.Core.Visuel
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;


    static class Preferences
    {
        public static GraphicsDeviceManager GraphicsDeviceManager   { get; set; }
        public static int WindowWidth                               { get; set; }
        public static int WindowHeight                              { get; set; }
        public static ContentManager Content                        { get; set; }
        public static float Brightness                              { get; set; }
        public static float Constrast                               { get; set; }
        public static ManagedThread ThreadParticles                 { get; set; }
        public static ManagedThread ThreadLogic                     { get; set; }
    }
}

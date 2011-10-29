namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Diagnostics;
    using Microsoft.Xna.Framework;


    public static class SilentErrorHandling
    {
        public static void Run<T>() where T : Game, new()
        {
            if (Debugger.IsAttached)
            {
                using (var g = new T())
                    g.Run();
            }

            else
            {
                try
                {
                    using (var g = new T())
                        g.Run();
                }

                catch (Exception e)
                {

                }
            }
        }
    }
}

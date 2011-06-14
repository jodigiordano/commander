namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    public static class ScreenSafeZone
    {
        static readonly Dictionary<float, Rectangle> prevValues = new Dictionary<float, Rectangle>();


        public static Rectangle GetXbox360()
        {
            return Get(0.8f);
        }


        public static Rectangle Get(float percent)
        {
            Rectangle retval;

            if (prevValues.TryGetValue(percent, out retval))
                return retval;

            retval = new Rectangle(
                Preferences.GraphicsDeviceManager.GraphicsDevice.Viewport.X,
                Preferences.GraphicsDeviceManager.GraphicsDevice.Viewport.Y,
                Preferences.GraphicsDeviceManager.GraphicsDevice.Viewport.Width,
                Preferences.GraphicsDeviceManager.GraphicsDevice.Viewport.Height);

            float border = (1 - percent) / 2;

            retval.X = (int) (border * retval.Width);
            retval.Y = (int) (border * retval.Height);
            retval.Width = (int) (percent * retval.Width);
            retval.Height = (int) (percent * retval.Height);

            prevValues.Add(percent, retval);

            return retval;
        }
    }
}

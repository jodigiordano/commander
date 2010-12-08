namespace Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Vibrator
    {
        private Dictionary<PlayerIndex, double> timeToVibrate = new Dictionary<PlayerIndex, double>();


        public Vibrator()
        {
            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
                timeToVibrate.Add(player, 0);
        }


        public void Initialize()
        {
            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
                timeToVibrate[player] = 0;
        }


        public void Vibrate(PlayerIndex player, double time, float left, float right)
        {
            timeToVibrate[player] = time;
            GamePad.SetVibration(player, left, right);
        }


        public void Update(GameTime gameTime)
        {
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                bool wasVibrating = timeToVibrate[i] > 0;

                timeToVibrate[i] = Math.Max(0, timeToVibrate[i] - gameTime.ElapsedGameTime.TotalMilliseconds);

                if (timeToVibrate[i] == 0 && wasVibrating)
                    GamePad.SetVibration(i, 0, 0);
            }
        }
    }
}

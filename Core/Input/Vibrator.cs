namespace EphemereGames.Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Vibrator
    {
        private Dictionary<PlayerIndex, double> TimesToVibrate = new Dictionary<PlayerIndex, double>(PlayerIndexComparer.Default);


        public Vibrator()
        {
            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
                TimesToVibrate.Add(player, 0);
        }


        public void Initialize()
        {
            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
            {
                TimesToVibrate[player] = 0;
                GamePad.SetVibration(player, 0, 0);
            }
        }


        public void Vibrate(PlayerIndex player, double time, float left, float right)
        {
            TimesToVibrate[player] = time;
            GamePad.SetVibration(player, left, right);
        }


        public void Update(GameTime gameTime)
        {
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                bool wasVibrating = TimesToVibrate[i] > 0;

                TimesToVibrate[i] = Math.Max(0, TimesToVibrate[i] - gameTime.ElapsedGameTime.TotalMilliseconds);

                if (TimesToVibrate[i] == 0 && wasVibrating)
                    GamePad.SetVibration(i, 0, 0);
            }
        }
    }
}

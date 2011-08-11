namespace EphemereGames.Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Vibrator
    {
        private Dictionary<PlayerIndex, double> TimesToVibrateLow;
        private Dictionary<PlayerIndex, double> TimesToVibrateHigh;

        private Dictionary<PlayerIndex, float> ForcesLow;
        private Dictionary<PlayerIndex, float> ForcesHigh;


        public Vibrator()
        {
            TimesToVibrateLow = new Dictionary<PlayerIndex, double>(PlayerIndexComparer.Default);
            TimesToVibrateHigh = new Dictionary<PlayerIndex, double>(PlayerIndexComparer.Default);
            ForcesLow = new Dictionary<PlayerIndex, float>(PlayerIndexComparer.Default);
            ForcesHigh = new Dictionary<PlayerIndex, float>(PlayerIndexComparer.Default);

            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
            {
                TimesToVibrateLow.Add(player, 0);
                TimesToVibrateHigh.Add(player, 0);

                ForcesLow.Add(player, 0);
                ForcesHigh.Add(player, 0);
            }
        }


        public void Initialize()
        {
            for (PlayerIndex player = PlayerIndex.One; player <= PlayerIndex.Four; player++)
            {
                TimesToVibrateLow[player] = 0;
                TimesToVibrateHigh[player] = 0;

                ForcesLow[player] = 0;
                ForcesHigh[player] = 0;

                GamePad.SetVibration(player, 0, 0);
            }
        }


        public void VibrateLowFrequency(PlayerIndex player, double time, float force)
        {
            TimesToVibrateLow[player] = time;
            ForcesLow[player] = force;

            GamePad.SetVibration(player, force, ForcesHigh[player]);
        }


        public void VibrateHighFrequency(PlayerIndex player, double time, float force)
        {
            TimesToVibrateHigh[player] = time;
            ForcesHigh[player] = force;

            GamePad.SetVibration(player, ForcesLow[player], force);
        }


        public void Update(GameTime gameTime)
        {
            VibrateLow(gameTime);
            VibrateHigh(gameTime);
        }


        private void VibrateLow(GameTime gameTime)
        {
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                bool wasVibrating = TimesToVibrateLow[i] > 0;

                TimesToVibrateLow[i] = Math.Max(0, TimesToVibrateLow[i] - gameTime.ElapsedGameTime.TotalMilliseconds);

                if (TimesToVibrateLow[i] == 0 && wasVibrating)
                {
                    ForcesLow[i] = 0;
                    GamePad.SetVibration(i, 0, ForcesHigh[i]);
                }
            }
        }


        private void VibrateHigh(GameTime gameTime)
        {
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                bool wasVibrating = TimesToVibrateHigh[i] > 0;

                TimesToVibrateHigh[i] = Math.Max(0, TimesToVibrateHigh[i] - gameTime.ElapsedGameTime.TotalMilliseconds);

                if (TimesToVibrateHigh[i] == 0 && wasVibrating)
                {
                    ForcesHigh[i] = 0;
                    GamePad.SetVibration(i, ForcesLow[i], 0);
                }
            }
        }
    }
}

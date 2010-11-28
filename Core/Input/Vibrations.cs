//==========================================================================
//
// Classe permettant de faire vibrer la manette du joueur à une force donnée
// pendant un temps donné.
//
// Toute nouvelle demande de vibrations écrase la précédente
//
//==========================================================================

namespace Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    class Vibrations
    {
        private static Vibrations instance = new Vibrations();

        private Dictionary<PlayerIndex, double> tempsVibration = new Dictionary<PlayerIndex, double>();    // temps de la vibration en ms
        private Dictionary<PlayerIndex, double> tempsFinVibration = new Dictionary<PlayerIndex, double>(); // temps représentant la fin de la vibration, -1 = non initialisé

        public void vibrer(PlayerIndex manette, double temps, float moteurGauche, float moteurDroit)
        {
            tempsFinVibration[manette] = 0;
            tempsVibration[manette] = temps;
            GamePad.SetVibration(manette, moteurGauche, moteurDroit);
        }

        public Vibrations()
        {
            tempsVibration.Add(PlayerIndex.One, 0);
            tempsVibration.Add(PlayerIndex.Two, 0);
            tempsVibration.Add(PlayerIndex.Three, 0);
            tempsVibration.Add(PlayerIndex.Four, 0);

            tempsFinVibration.Add(PlayerIndex.One, -1);
            tempsFinVibration.Add(PlayerIndex.Two, -1);
            tempsFinVibration.Add(PlayerIndex.Three, -1);
            tempsFinVibration.Add(PlayerIndex.Four, -1);
        }

        public static Vibrations Instance
        {
            get { return instance; }
        }

        public void Update(GameTime gameTime)
        {
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++)
            {
                if (tempsFinVibration[i] == 0)
                    tempsFinVibration[i] = gameTime.TotalGameTime.TotalMilliseconds + tempsVibration[i];

                else if (tempsFinVibration[i] > 0 && gameTime.TotalGameTime.TotalMilliseconds >= tempsFinVibration[i])
                {
                    // arrêt des vibrations
                    GamePad.SetVibration(i, 0, 0);

                    tempsFinVibration[i] = -1;
                }
            }
        }
    }
}

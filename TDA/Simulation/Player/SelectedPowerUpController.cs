namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class SelectedPowerUpController
    {
        public PowerUpType PowerUpToBuy;

        private Dictionary<PowerUpType, PowerUp> PowerUps;
        private Dictionary<PowerUpType, Cercle> PowerUpsCircles;
        private Cercle CursorCircle;


        public SelectedPowerUpController(Dictionary<PowerUpType, PowerUp> powerUps, Cercle cursorCircle)
        {
            PowerUps = powerUps;
            CursorCircle = cursorCircle;
            PowerUpToBuy = PowerUpType.None;

            PowerUpsCircles = new Dictionary<PowerUpType, Cercle>();

            foreach (var powerUp in PowerUps)
                PowerUpsCircles.Add(powerUp.Key, new Cercle(powerUp.Value.BuyPosition, 10));
        }


        public void UpdateSelection()
        {
            PowerUpToBuy = PowerUpType.None;

            foreach (var powerUp in PowerUpsCircles)
            {
                powerUp.Value.Position = PowerUps[powerUp.Key].BuyPosition;

                if (Core.Physique.Facade.collisionCercleCercle(powerUp.Value, CursorCircle))
                    PowerUpToBuy = powerUp.Key;
            }
        }
    }
}

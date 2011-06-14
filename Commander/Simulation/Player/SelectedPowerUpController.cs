namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.Physics;

    class SelectedPowerUpController
    {
        public PowerUpType PowerUpToBuy;

        private Dictionary<PowerUpType, PowerUp> PowerUps;
        private Dictionary<PowerUpType, Circle> PowerUpsCircles;
        private Circle CursorCircle;


        public SelectedPowerUpController(Dictionary<PowerUpType, PowerUp> powerUps, Circle cursorCircle)
        {
            PowerUps = powerUps;
            CursorCircle = cursorCircle;
            PowerUpToBuy = PowerUpType.None;

            PowerUpsCircles = new Dictionary<PowerUpType, Circle>(PowerUpTypeComparer.Default);

            foreach (var powerUp in PowerUps)
                PowerUpsCircles.Add(powerUp.Key, new Circle(powerUp.Value.BuyPosition, 10));
        }


        public void UpdateSelection()
        {
            PowerUpToBuy = PowerUpType.None;

            foreach (var powerUp in PowerUpsCircles)
            {
                powerUp.Value.Position = PowerUps[powerUp.Key].BuyPosition;

                if (Core.Physics.Physics.collisionCercleCercle(powerUp.Value, CursorCircle))
                    PowerUpToBuy = powerUp.Key;
            }
        }
    }
}

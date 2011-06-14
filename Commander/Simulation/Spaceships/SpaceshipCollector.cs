namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.Physics;


    class SpaceshipCollector : SpaceshipSpaceship
    {
        public SpaceshipCollector(Simulation simulation)
            : base(simulation)
        {
            Image = new Image("Collecteur")
            {
                SizeX = 4
            };

            RotationMaximaleRad = 0.2f;
            BuyPrice = 0;
            ShootingFrequency = double.NaN;
            ActiveTime = double.MaxValue;
            Active = true;
            SfxOut = "sfxPowerUpCollecteurOut";
            SfxIn = "sfxPowerUpCollecteurIn";
            Active = true;
            ShowTrail = true;
        }


        public override bool Active { get; set; }
    }
}

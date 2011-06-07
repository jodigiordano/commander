namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    class SpaceshipMiner : SpaceshipSpaceship
    {
        public SpaceshipMiner(Simulation simulation)
            : base(simulation)
        {
            Image = new Image("Resistance3")
            {
                SizeX = 4
            };

            RotationMaximaleRad = 0.2f;
            BuyPrice = 0;
            ShootingFrequency = double.NaN;
            ActiveTime = double.MaxValue;
            Active = true;
            SfxOut = "sfxMinerOut";
            SfxIn = "sfxMinerIn";
            Active = true;
        }


        public override bool Active { get; set; }
    }
}

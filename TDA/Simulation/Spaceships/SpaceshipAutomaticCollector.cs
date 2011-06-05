namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    class SpaceshipAutomaticCollector : SpaceshipSpaceship
    {
        public List<Mineral> Minerals;


        public SpaceshipAutomaticCollector(Simulation simulation)
            : base(simulation)
        {
            Image = new Image("Collecteur")
            {
                SizeX = 4
            };

            RotationMaximaleRad = 0.3f;
            BuyPrice = 0;
            ShootingFrequency = double.NaN;
            ActiveTime = double.MaxValue;
            Active = true;
            SfxGoHome = "sfxPowerUpCollecteurOut";
            SfxIn = "sfxPowerUpCollecteurIn";
            Active = true;
            AutomaticMode = true;
        }


        public override void Update()
        {
            if (!InCombat)
            {
                if (Minerals.Count == 0)
                    TargetPosition = new Vector3(
                        Main.Random.Next(Simulation.InnerTerrain.Left, Simulation.InnerTerrain.Right),
                        Main.Random.Next(Simulation.InnerTerrain.Top, Simulation.InnerTerrain.Bottom),
                        0);
                else
                    TargetPosition = Minerals[0].Position;
            }

            base.Update();
        }
    }
}

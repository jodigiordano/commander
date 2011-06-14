﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.Physics;


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
            SfxOut = "sfxAutomaticCollectorOut";
            SfxIn = "sfxAutomaticCollectorIn";
            Active = true;
            AutomaticMode = false;
        }


        public override void Update()
        {
            if (float.IsNaN(Position.X))
                Position = Vector3.Zero;

            if (float.IsNaN(TargetPosition.X))
                InCombat = false;

            if (!InCombat)
            {
                if (Minerals.Count == 0)
                    TargetPosition = new Vector3(
                        Main.Random.Next(Simulation.InnerTerrain.Left + 50, Simulation.InnerTerrain.Right - 50),
                        Main.Random.Next(Simulation.InnerTerrain.Top + 50, Simulation.InnerTerrain.Bottom - 50),
                        0);
                else
                    TargetPosition = Minerals[0].Position;
            }

            NextInput = TargetPosition - Position;
            NextInput.Normalize();
            NextInput *= 3;

            if ((TargetPosition - Position).LengthSquared() <= 600)
            {
                InCombat = false;
                TargetReached = true;
            }

            base.Update();

            Direction = NextInput;
        }
    }
}

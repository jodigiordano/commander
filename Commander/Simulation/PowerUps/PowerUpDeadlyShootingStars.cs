namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpDeadlyShootingStars : PowerUp
    {
        public Circle DestructionRange          { get; private set; }
        public int AttackPoints                 { get; private set; }


        public PowerUpDeadlyShootingStars(Simulator simulation)
            : base(simulation)
        {
            DestructionRange = new Circle(new Vector3(), 100);
            AttackPoints = 50000;
            Type = PowerUpType.DeadlyShootingStars;
            Category = PowerUpCategory.Other;
            BuyImage = "shootingStars";
            BuyPrice = 500;
            BuyTitle = "Deadly Shooting Stars (" + BuyPrice + "M$)";
            BuyDescription = "The shooting stars become deadly to enemies.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride; }
        }


        public override void Start()
        {
            Audio.PlaySfx(@"Partie", @"sfxShootingStars");
        }
    }
}

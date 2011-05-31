namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpDeadlyShootingStars : PowerUp
    {
        public Cercle DestructionRange          { get; private set; }
        public int AttackPoints                 { get; private set; }


        public PowerUpDeadlyShootingStars(Simulation simulation)
            : base(simulation)
        {
            DestructionRange = new Cercle(new Vector3(), 100);
            AttackPoints = 50000;
            Type = PowerUpType.DeadlyShootingStars;
            Category = PowerUpCategory.Other;
            BuyImage = "Destruction";
            BuyPrice = 500;
            BuyTitle = "Deadly Shooting Stars";
            BuyDescription = "The shooting stars become deadly to enemies.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return false; }
        }


        public override void Start()
        {
            
        }
    }
}

namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SpaceshipAutomaticCollector : SpaceshipSpaceship
    {
        public List<Mineral> Minerals;


        public SpaceshipAutomaticCollector(Simulator simulator)
            : base(simulator)
        {
            Image = new Image("Collecteur")
            {
                SizeX = 4
            };

            BuyPrice = 0;
            Active = true;
            SfxOut = "sfxAutomaticCollectorOut";
            SfxIn = "sfxAutomaticCollectorIn";
            Active = true;
        }


        public void Initialize()
        {
            SteeringBehavior = new SpaceshipChasingABehavior(this, NextPosition());
        }


        public override void Update()
        {
            if (!SteeringBehavior.Active)
                ((SpaceshipChasingABehavior) SteeringBehavior).TargetPosition = NextPosition();

            base.Update();
        }


        private Vector3 NextPosition()
        {
            return (Minerals.Count == 0) ?
                new Vector3(
                    Main.Random.Next(Simulator.InnerTerrain.Left + 50, Simulator.InnerTerrain.Right - 50),
                    Main.Random.Next(Simulator.InnerTerrain.Top + 50, Simulator.InnerTerrain.Bottom - 50),
                    0) :
                Minerals[0].Position;
        }
    }
}

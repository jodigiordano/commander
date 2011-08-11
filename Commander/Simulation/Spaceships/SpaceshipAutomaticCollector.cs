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

            MaxRotationRad = 0.3f;
            BuyPrice = 0;
            Active = true;
            SfxOut = "sfxAutomaticCollectorOut";
            SfxIn = "sfxAutomaticCollectorIn";
            Active = true;
            ApplyAutomaticBehavior = false;
        }


        public void Initialize()
        {
            AutomaticBehavior = new SpaceshipChasingBehavior(this, NextPosition());
        }


        public override void Update()
        {
            if (!AutomaticBehavior.Active)
                ((SpaceshipChasingBehavior) AutomaticBehavior).TargetPosition = NextPosition();

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

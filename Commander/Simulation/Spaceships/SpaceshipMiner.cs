namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class SpaceshipMiner : SpaceshipSpaceship
    {
        private List<Bullet> ToAdd;


        public SpaceshipMiner(Simulator simulation)
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
            ShowTrail = true;

            ToAdd = new List<Bullet>();
        }


        public void Fire()
        {
            MineBullet mb = (MineBullet) Simulation.BulletsFactory.Get(BulletType.Mine);

            mb.Position = Position;
            mb.AttackPoints = BulletHitPoints;
            mb.VisualPriority = Preferences.PrioriteSimulationChemin - 0.001f;
            mb.Speed = 0;
            mb.ExplosionRange = 70;

            ToAdd.Add(mb);
        }


        public override List<Bullet> BulletsThisTick()
        {
            Bullets.Clear();
            Bullets.AddRange(ToAdd);
            ToAdd.Clear();

            return Bullets;
        }


        public override bool Active { get; set; }
    }
}

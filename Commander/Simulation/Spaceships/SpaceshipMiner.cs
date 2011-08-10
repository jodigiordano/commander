namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class SpaceshipMiner : SpaceshipSpaceship
    {
        public SimPlayer Owner;

        private List<Bullet> ToAdd;


        public SpaceshipMiner(Simulator simulator)
            : base(simulator)
        {
            Image = new Image("Resistance3")
            {
                SizeX = 4
            };

            RotationMaximaleRad = 0.2f;
            BuyPrice = 0;
            ShootingFrequency = double.NaN;
            Active = true;
            SfxOut = "sfxMinerOut";
            SfxIn = "sfxMinerIn";
            Active = true;
            ShowTrail = true;

            ToAdd = new List<Bullet>();
        }


        public void Fire()
        {
            MineBullet mb = (MineBullet) Simulator.BulletsFactory.Get(BulletType.Mine);

            mb.Position = Position;
            mb.AttackPoints = BulletHitPoints;
            mb.VisualPriority = VisualPriorities.Default.MineBullet;
            mb.Speed = 0;
            mb.ExplosionRange = 70;
            mb.Owner = Owner;

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

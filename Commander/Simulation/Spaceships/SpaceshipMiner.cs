namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.Physics;


    class SpaceshipMiner : SpaceshipSpaceship
    {
        private List<Bullet> ToAdd;


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

namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Audio;


    class PowerUpPulse : PowerUp
    {
        public Path Path;
        public PulseBullet Bullet;

        private double TravelTime;
        private Vector3 BulletPosition;


        public PowerUpPulse(Simulator simulation)
            : base(simulation)
        {
            Type = PowerUpType.Pulse;
            Category = PowerUpCategory.Other;
            BuyImage = "pulse";
            BuyPrice = 500;
            BuyTitle = "Plasma Pulse (" + BuyPrice + "M$)";
            BuyDescription = "A deadly pulse is sent on the path and strikes every enemy.";
            NeedInput = false;
            Position = Vector3.Zero;
            TravelTime = 0;
            BulletPosition = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || TravelTime >= Path.Length; }
        }


        public override void Update()
        {
            TravelTime += Bullet.Speed;

            if (TravelTime % 1000 == 0)
                Audio.PlaySfx(@"Partie", @"sfxPulse2");
            
            Path.Position(Path.Length - TravelTime, ref BulletPosition);

            Bullet.Position = BulletPosition;
        }


        public override void Start()
        {
            Bullet = (PulseBullet) Simulation.BulletsFactory.Get(BulletType.Pulse);

            Bullet.AttackPoints = 50;
            Bullet.LifePoints = float.MaxValue;
            Bullet.Speed = 10;
            Bullet.VisualPriority = Preferences.PrioriteSimulationEnnemi - 0.0001f;

            Bullet.Initialize();

            Audio.PlaySfx(@"Partie", @"sfxPulse1");
        }


        public override void Stop()
        {
            Audio.PlaySfx(@"Partie", @"sfxPulse3");
        }
    }
}

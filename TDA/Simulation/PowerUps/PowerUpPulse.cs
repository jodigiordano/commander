namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpPulse : PowerUp
    {
        public Path Path;
        public PulseBullet Bullet;

        private double TravelTime;
        private Vector3 BulletPosition;


        public PowerUpPulse(Simulation simulation)
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
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPulse2");
            
            Path.Position(Path.Length - TravelTime, ref BulletPosition);

            Bullet.Position = BulletPosition;
        }


        public override void Start()
        {
            Bullet = new PulseBullet()
            {
                Scene = Simulation.Scene,
                AttackPoints = 50,
                LifePoints = float.MaxValue,
                Speed = 10,
                PrioriteAffichage = Preferences.PrioriteSimulationEnnemi - 0.0001f
            };

            Bullet.Initialize();

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPulse1");
        }


        public override void Stop()
        {
            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPulse3");
        }
    }
}

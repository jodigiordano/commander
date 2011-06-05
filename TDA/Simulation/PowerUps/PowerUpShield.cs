namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class PowerUpShield : PowerUp
    {
        public ShieldBullet Bullet;
        public CorpsCeleste ToProtect;

        private double ActiveTime;


        public PowerUpShield(Simulation simulation)
            : base(simulation)
        {
            Type = PowerUpType.Shield;
            Category = PowerUpCategory.Other;
            BuyImage = "shield";
            BuyPrice = 500;
            ActiveTime = 10000;
            BuyTitle = "Shield (" + BuyPrice + "M$)";
            BuyDescription = "Become temporary invincible for " + (int) ActiveTime / 1000 + " sec.";
            NeedInput = false;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || ActiveTime <= 0; }
        }


        public override void Update()
        {
            Bullet.Position = ToProtect.Position;
            Bullet.Rectangle.Width = (int) (ToProtect.Cercle.Radius * 2 + 10);
            Bullet.Rectangle.Height = (int) (ToProtect.Cercle.Radius * 2 + 10);
            ((CircleEmitter) Bullet.MovingEffect.ParticleEffect[0]).Radius = ToProtect.Cercle.Radius;
            ((RadialGravityModifier) Bullet.MovingEffect.ParticleEffect[0].Modifiers[0]).Radius = ToProtect.Cercle.Radius + 10;
            ((RadialGravityModifier) Bullet.MovingEffect.ParticleEffect[0].Modifiers[0]).Position = new Vector2(ToProtect.position.X, ToProtect.position.Y);

            ActiveTime -= 16.66;
        }


        public override void Start()
        {
            Bullet = new ShieldBullet()
            {
                Scene = Simulation.Scene,
                AttackPoints = float.MaxValue,
                LifePoints = float.MaxValue,
                Vitesse = 0,
                PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.0001f
            };

            Bullet.Initialize();
        }
    }
}

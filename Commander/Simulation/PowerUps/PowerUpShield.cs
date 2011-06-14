namespace EphemereGames.Commander
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class PowerUpShield : PowerUp
    {
        public ShieldBullet Bullet;
        public CelestialBody ToProtect;

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
            Bullet.Rectangle.Width = (int) (ToProtect.Circle.Radius * 2 + 10);
            Bullet.Rectangle.Height = (int) (ToProtect.Circle.Radius * 2 + 10);
            ((CircleEmitter) Bullet.MovingEffect.ParticleEffect[0]).Radius = ToProtect.Circle.Radius;

            ActiveTime -= Preferences.TargetElapsedTimeMs;
        }


        public override void Start()
        {
            Bullet = (ShieldBullet) Simulation.BulletsFactory.Get(BulletType.Shield);

            Bullet.AttackPoints = float.MaxValue;
            Bullet.LifePoints = float.MaxValue;
            Bullet.Speed = 0;
            Bullet.VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.0001f;

            Bullet.Initialize();

            Audio.PlaySfx(@"Partie", @"sfxShieldIn");
        }


        public override void Stop()
        {
            Audio.PlaySfx(@"Partie", @"sfxShieldOut");
        }
    }
}
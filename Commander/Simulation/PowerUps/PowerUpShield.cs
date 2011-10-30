namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class PowerUpShield : PowerUp
    {
        public ShieldBullet Bullet;
        public CelestialBody ToProtect;

        private double ActiveTime;


        public PowerUpShield(Simulator simulator)
            : base(simulator)
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
            SfxIn = "sfxShieldIn";
            SfxOut = "sfxShieldOut";
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
            ((CircleEmitter) Bullet.MovingEffect.Model[0]).Radius = ToProtect.Circle.Radius;

            ActiveTime -= Preferences.TargetElapsedTimeMs;
        }


        public override void Start()
        {
            Bullet = (ShieldBullet) Simulator.BulletsFactory.Get(BulletType.Shield);

            Bullet.AttackPoints = float.MaxValue;
            Bullet.LifePoints = float.MaxValue;
            Bullet.Speed = 0;
            Bullet.VisualPriority = VisualPriorities.Default.PowerUpShield;

            Bullet.Initialize();
        }
    }
}

namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class PowerUpSniper : PowerUp
    {
        private HumanBattleship HumanBattleship;
        private SniperTurret Turret;
        private bool terminated;


        public PowerUpSniper(Simulator simulator, HumanBattleship humanBattleship)
            : base(simulator)
        {
            HumanBattleship = humanBattleship;
            Type = PowerUpType.Sniper;
            Category = PowerUpCategory.Turret;
            PayOnActivation = false;
            PayOnUse = true;
            BuyImage = "Destruction";
            UsePrice = 50;
            BuyTitle = "Sniper (" + UsePrice + "M$ per shot)";
            BuyDescription = "Controls a sniper gun that kill (or miss) with extreme precision.";
            NeedInput = true;
            Crosshair = "crosshairSniper";
            CrosshairSize = 2;
            Position = Vector3.Zero;
            AssociatedTurret = TurretType.Sniper;

            terminated = false;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || terminated; }
        }


        public override void DoInputCanceled()
        {
            terminated = true;
            HumanBattleship.Sniper.Wander = true;
        }


        public override void DoInputMoved(Vector3 position)
        {
            base.DoInputMoved(position);

            Vector3 direction = position - HumanBattleship.Sniper.Position;

            HumanBattleship.Sniper.Rotation = MathHelper.PiOver2 + (float) Math.Atan2(direction.Y, direction.X);
            HumanBattleship.Sniper.Direction = direction;
        }


        public override void Start()
        {
            terminated = false;
            HumanBattleship.Sniper.Wander = false;
            Turret = HumanBattleship.Sniper;
        }
    }
}

namespace EphemereGames.Commander.Simulation
{
    using System;
    using Microsoft.Xna.Framework;


    class PowerUpRailGun : PowerUp
    {
        private PowerUpsBattleship HumanBattleship;
        private RailGunTurret Turret;
        private bool terminated;

        private bool Firing;


        public PowerUpRailGun(Simulator simulator, PowerUpsBattleship humanBattleship)
            : base(simulator)
        {
            HumanBattleship = humanBattleship;

            Type = PowerUpType.RailGun;
            Category = PowerUpCategory.Turret;
            PayOnActivation = false;
            PayOnUse = true;
            BuyImage = "";
            BuyPrice = 0;
            UsePrice = 100;
            BuyTitle = "The railgun (" + UsePrice + "M$ per shot)";
            BuyDescription = "Control a poweful, yet unstable, deadly weapon!";
            NeedInput = true;
            Crosshair = "crosshairRailGun";
            Position = Vector3.Zero;
            AssociatedTurret = TurretType.RailGun;
            SfxIn = "sfxRailGunIn";
            SfxOut = "sfxRailGunOut";
        }


        public override bool Terminated
        {
            get { return TerminatedOverride || terminated; }
        }


        public override void DoInputReleased()
        {
            if (Firing)
            {
                Turret.StopFire();
                Firing = false;
            }
        }


        public override void DoInputCanceled()
        {
            terminated = true;

            DoInputReleased();
        }


        public override void DoInputPressed()
        {
            Turret.Fire();
            Firing = true;
        }


        public override void DoInputMoved(Vector3 position)
        {
            base.DoInputMoved(position);

            Vector3 direction = position - HumanBattleship.RailGun.Position;

            HumanBattleship.RailGun.Rotation = MathHelper.PiOver2 + (float) Math.Atan2(direction.Y, direction.X);
            HumanBattleship.RailGun.Direction = direction;
        }


        public override void Start()
        {
            terminated = false;
            HumanBattleship.RailGun.Wander = false;
            Turret = HumanBattleship.RailGun;
            Turret.Owner = Owner;
            Firing = false;
        }


        public override void Stop()
        {
            Turret.StopFire();
            Firing = false;
            HumanBattleship.RailGun.Wander = true;
        }
    }
}

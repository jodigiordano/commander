namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;


    class PowerUpRailGun : PowerUp
    {
        private HumanBattleship HumanBattleship;
        private RailGunTurret Turret;
        private bool terminated;

        private bool Firing;


        public PowerUpRailGun(Simulation simulation, HumanBattleship humanBattleship)
            : base(simulation)
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

                EphemereGames.Core.Audio.Facade.arreterEffetSonore("Partie", "sfxRailGunCharging");
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

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxRailGunCharging");
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
            Firing = false;

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxRailGunIn");
        }


        public override void Stop()
        {
            Turret.StopFire();
            Firing = false;
            HumanBattleship.RailGun.Wander = true;

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxRailGunOut");
        }
    }
}

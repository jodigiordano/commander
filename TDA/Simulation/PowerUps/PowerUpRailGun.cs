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
            BuyImage = "";
            BuyPrice = 0;
            BuyTitle = "The railgun";
            BuyDescription = "Control a poweful, yet unstable, deadly weapon!";
            NeedInput = true;
            Position = Vector3.Zero;
        }


        public override bool Terminated
        {
            get { return terminated; }
        }


        public override void Update()
        {

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
            HumanBattleship.RailGun.Wander = true;

            DoInputReleased();
        }


        public override void DoInputPressed()
        {
            Turret.Fire();
            Firing = true;
        }


        public override void DoInputMoved(Vector3 position)
        {
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
        }
    }
}

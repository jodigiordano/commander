namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class RailGunTurret : Turret
    {
        public SimPlayer Owner;


        public RailGunTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.RailGun;
            Name = "RailGun";
            SfxShooting = "sfxRailGunExplosion1";
            Color = new Color(57, 216, 17);
            BackActiveThisTickOverride = true;

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 10000, 1, 0, BulletType.RailGun, "", "", 0, 0, 0),
                new TurretLevel(1, 30, 15, 75, 3000, 1, 0, BulletType.RailGun, "railGunTurretCanon", "railGunTurretBase", 1000, 100, 15)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override List<Bullet> BulletsThisTick()
        {
            var result = base.BulletsThisTick();

            foreach (var bullet in result)
                ((RailGunBullet) bullet).Owner = Owner;

            return result;
        }
    }
}
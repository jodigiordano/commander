namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class RailGunTurret : Turret
    {
        public SimPlayer Owner;


        public RailGunTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.RailGun;
            Name = @"RailGun";
            Description = @"";
            SfxShooting = @"sfxRailGunExplosion1";
            Color = new Color(57, 216, 17);
            BackActiveThisTickOverride = true;

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

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
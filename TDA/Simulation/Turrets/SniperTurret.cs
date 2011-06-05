namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class SniperTurret : Turret
    {
        public SniperTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.Sniper;
            Name = "Sniper";
            SfxShooting = "sfxTourelleBase";
            Color = new Color(57, 216, 17);
            BackActiveThisTickOverride = true;

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 10000, 1, 0, BulletType.RailGun, "", "", 0, 0, 0),
                new TurretLevel(1, 30, 15, 75, 3000, 1, 0, BulletType.RailGun, "sniperTurretCanon", "sniperTurretBase", 1000, 100, 15)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override bool Upgrade()
        {
            if (base.Upgrade())
            {
                CanonImage.Origin = new Vector2(6, 12);
                return true;
            }

            return false;
        }
    }
}
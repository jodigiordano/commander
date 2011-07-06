namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class SniperTurret : Turret
    {
        public SniperTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.Sniper;
            Name = "Sniper";
            SfxShooting = "sfxTourelleBase";
            Color = new Color(57, 216, 17);
            BackActiveThisTickOverride = true;

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

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
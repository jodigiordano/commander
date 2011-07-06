namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class MissileTurret : Turret
    {
        public MissileTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.Missile;
            Name = "Missile";
            SfxShooting = "sfxTourelleMissile";
            Color = new Color(25, 121, 255);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override double VisualPriority
        {
            set
            {
                base.VisualPriority = value;

                CanonImage.VisualPriority = BaseImage.VisualPriority - 0.00001f;
            }
        }
    }
}
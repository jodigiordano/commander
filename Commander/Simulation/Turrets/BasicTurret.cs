namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class BasicTurret : Turret
    {
        public BasicTurret(Simulator simulator)
            : base(simulator)
        {
            Type = TurretType.Basic;
            Name = "Basic";
            SfxShooting = "sfxTourelleBase";
            Color = new Color(57, 216, 17);

            Levels = simulator.TurretsFactory.TurretsLevels[Type];

            ActualLevel = Levels.First;
            Upgrade();
        }
    }
}
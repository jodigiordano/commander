namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class AlienTurret : GravitationalTurret
    {
        public AlienTurret(Simulation simulation)
            : base(simulation)
        {
            Name = "Alien";

            AntennaRotationSpeed = Main.Random.Next(-50, 50) / 1000f;
            AntennaRotationBase = 0;
            Color = new Color(202, 196, 255);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 1, Int16.MaxValue, 1, 0, BulletType.Aucun, "", "", 0, 0, 0),
                new TurretLevel(1, 1000, 500, 1, Int16.MaxValue, 1, 1000, BulletType.Aucun, "tourelleAlien", "tourelleAlienBase", 0, 0, 0)
            };

            Upgrade();
        }


        public override bool Upgrade()
        {
            if (base.Upgrade())
            {
                AntennaRotationBase = (AntennaRotationSpeed > 0) ? -0.02f : 0.02f;
                CanonImage.Color.A = 60;
                BaseImage.Color.A = 60;
                return true;
            }

            return false;
        }
    }
}
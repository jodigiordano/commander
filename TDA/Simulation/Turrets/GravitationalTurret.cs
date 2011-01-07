namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class GravitationalTurret : Turret
    {
        public float AntennaRotationSpeed;
        public float AntennaRotationBase;


        public GravitationalTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.Gravitational;
            Name = "Gravitational";

            AntennaRotationSpeed = Main.Random.Next(-50, 50) / 1000f;
            AntennaRotationBase = 0;
            Color = new Color(202, 196, 255);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, new Cercle(Vector3.Zero, 1), Int16.MaxValue, 1, 0, BulletType.Aucun, "", "", 0, null, 0),
                new TurretLevel(1, 1000, 500, new Cercle(Vector3.Zero, 1), Int16.MaxValue, 1, 500, BulletType.Aucun, "tourelleGravitationnelleAntenne", "tourelleGravitationnelleBase", 0, null, 0),
                new TurretLevel(2, 500, 750, new Cercle(Vector3.Zero, 1), Int16.MaxValue, 1, 500, BulletType.Aucun, "tourelleGravitationnelleAntenne", "tourelleGravitationnelleBase", 0, null, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override void Draw()
        {
            CanonImage.Rotation += AntennaRotationSpeed;
            BaseImage.Rotation += AntennaRotationBase;

            base.Draw();
        }


        public override float VisualPriority
        {
            set
            {
                base.VisualPriority = value;

                CanonImage.VisualPriority = BaseImage.VisualPriority - 0.001f;
            }
        }


        public override bool Upgrade()
        {
            if (base.Upgrade())
            {
                CanonImage.Origine = CanonImage.Centre;
                return true;
            }

            return false;
        }
    }
}
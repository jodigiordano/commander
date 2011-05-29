namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class MissileTurret : Turret
    {
        public MissileTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.Missile;
            Name = "Missile";
            SfxShooting = "sfxTourelleMissile";
            Color = new Color(25, 121, 255);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 150, 2600, 1, 0, BulletType.Missile, "", "", 30, 50, 1.8f),
                new TurretLevel(1, 150, 75, 150, 2600, 1, 3000, BulletType.Missile, "tourelleMissileCanon1", "tourelleMissileBase", 30, 50, 1.8f),
                new TurretLevel(2, 100, 125, 200, 2400, 1, 5000, BulletType.Missile, "tourelleMissileCanon1", "tourelleMissileBase", 40, 60, 2.0f),
                new TurretLevel(3, 150, 200, 250, 2200, 1, 7000, BulletType.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 50, 70, 2.2f),
                new TurretLevel(4, 200, 300, 275, 2000, 1, 9000, BulletType.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 60, 80, 2.4f),
                new TurretLevel(5, 250, 425, 300, 1800, 1, 11000, BulletType.Missile, "tourelleMissileCanon2", "tourelleMissileBase", 70, 90, 2.6f),
                new TurretLevel(6, 300, 575, 325, 1600, 1, 13000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 80, 100, 2.8f),
                new TurretLevel(7, 350, 750, 350, 1400, 1, 15000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 90, 110, 3.0f),
                new TurretLevel(8, 400, 950, 400, 1200, 1, 17000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 100, 120, 3.2f),
                new TurretLevel(9, 450, 1175, 425, 1000, 1, 19000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 110, 130, 3.4f),
                new TurretLevel(10, 500, 1425, 450, 800, 1, 21000, BulletType.Missile2, "tourelleMissileCanon3", "tourelleMissileBase", 120, 140, 3.6f)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }


        public override float VisualPriority
        {
            set
            {
                base.VisualPriority = value;

                CanonImage.VisualPriority = BaseImage.VisualPriority - 0.001f;
            }
        }
    }
}
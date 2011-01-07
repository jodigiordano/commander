namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class BasicTurret : Turret
    {
        public BasicTurret(Simulation simulation)
            : base(simulation)
        {
            Type = TurretType.Basic;
            Name = "Basic";
            SfxShooting = "sfxTourelleBase";
            Color = new Color(57, 216, 17);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, new Cercle(Vector3.Zero, 0), 10000, 1, 0, BulletType.Base, "", "", 0, null, 0),
                new TurretLevel(1, 30, 15, new Cercle(Vector3.Zero, 75), 650, 1, 1000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 5, null, 0),
                new TurretLevel(2, 60, 45, new Cercle(Vector3.Zero, 100), 600, 1, 2000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 7, null, 0),
                new TurretLevel(3, 90, 90, new Cercle(Vector3.Zero, 125), 550, 1, 3000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 10, null, 0),
                new TurretLevel(4, 120, 150, new Cercle(Vector3.Zero, 150), 500, 2, 4000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 7, null, 0),
                new TurretLevel(5, 150, 225, new Cercle(Vector3.Zero, 175), 450, 2, 5000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 10, null, 0),
                new TurretLevel(6, 180, 315, new Cercle(Vector3.Zero, 200), 400, 2, 6000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 12, null, 0),
                new TurretLevel(7, 210, 420, new Cercle(Vector3.Zero, 225), 350, 2, 7000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 14, null, 0),
                new TurretLevel(8, 240, 540, new Cercle(Vector3.Zero, 250), 300, 3, 8000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 9, null, 0),
                new TurretLevel(9, 270, 675, new Cercle(Vector3.Zero, 275), 250, 3, 9000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 11, null, 0),
                new TurretLevel(10, 300, 825, new Cercle(Vector3.Zero, 300), 200, 3, 10000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 12, null, 0)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }
    }
}
namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class BasicTurret : Turret
    {
        public BasicTurret(Simulator simulation)
            : base(simulation)
        {
            Type = TurretType.Basic;
            Name = "Basic";
            SfxShooting = "sfxTourelleBase";
            Color = new Color(57, 216, 17);

            Levels = new LinkedListWithInit<TurretLevel>()
            {
                new TurretLevel(0, 0, 0, 0, 10000, 1, 0, BulletType.Base, "", "", 0, 0, 0),
                new TurretLevel(1, 30, 15, 75, 650, 1, 1000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 5, 0, 10),
                new TurretLevel(2, 60, 45, 100, 600, 1, 2000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 7, 0, 10),
                new TurretLevel(3, 90, 90, 125, 550, 1, 3000, BulletType.Base, "tourelleBase1", "tourelleBaseBase", 10, 0, 10),
                new TurretLevel(4, 120, 150, 150, 500, 2, 4000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 7, 0, 10),
                new TurretLevel(5, 150, 225, 175, 450, 2, 5000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 10, 0, 10),
                new TurretLevel(6, 180, 315, 200, 400, 2, 6000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 12, 0, 10),
                new TurretLevel(7, 210, 420, 225, 350, 2, 7000, BulletType.Base, "tourelleBase2", "tourelleBaseBase", 14, 0, 10),
                new TurretLevel(8, 240, 540, 250, 300, 3, 8000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 9, 0, 10),
                new TurretLevel(9, 270, 675, 275, 250, 3, 9000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 11, 0, 10),
                new TurretLevel(10, 300, 825, 300, 200, 3, 10000, BulletType.Base, "tourelleBase3", "tourelleBaseBase", 12, 0, 10)
            };

            ActualLevel = Levels.First;
            Upgrade();
        }
    }
}
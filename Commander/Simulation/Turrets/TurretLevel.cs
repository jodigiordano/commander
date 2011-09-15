namespace EphemereGames.Commander.Simulation
{

    class TurretLevel
    {
        public int Level;
        public int BuyPrice;
        public int SellPrice;
        public float Range;
        public double FireRate;
        public int NbCanons;
        public double BuildingTime;
        public BulletType Bullet;
        public string CanonImageName;
        public string BaseImageName;
        public float BulletHitPoints;
        public float BulletExplosionRange;
        public float BulletSpeed;
        public string MovingSfx;
        public string FiringSfx;
        public string BulletExplosionSfx;


        public TurretLevel()
        {
            Level = 1;
            BuyPrice = 0;
            SellPrice = 0;
            Range = 1;
            FireRate = 1;
            NbCanons = 1;
            BuildingTime = 1;
            Bullet = BulletType.None;
            CanonImageName = "";
            BaseImageName = "";
            BulletHitPoints = 0;
            BulletExplosionRange = 1;
            BulletSpeed = 0;
            MovingSfx = "";
            FiringSfx = "";
            BulletExplosionSfx = "";
        }


        public TurretLevel(
            int level,
            int buyPrice,
            int sellPrice,
            float range,
            double fireRate,
            int nbCanons,
            double buildingTime,
            BulletType bullet,
            string canonImageName,
            string baseImageName,
            float bulletHitPoints,
            float bulletSpeed,
            string movingSfx,
            string firingSfx)
        {
            Level = level;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
            Range = range;
            FireRate = fireRate;
            NbCanons = nbCanons;
            BuildingTime = buildingTime;
            Bullet = bullet;
            CanonImageName = canonImageName;
            BaseImageName = baseImageName;
            BulletHitPoints = bulletHitPoints;
            BulletSpeed = bulletSpeed;
            MovingSfx = movingSfx;
            FiringSfx = firingSfx;
        }
    }
}

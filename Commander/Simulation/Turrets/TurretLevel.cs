namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;

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


        public TurretLevel()
        {
            Level = 1;
            BuyPrice = 0;
            SellPrice = 0;
            Range = 1;
            FireRate = 1;
            NbCanons = 1;
            BuildingTime = 1;
            Bullet = BulletType.Aucun;
            CanonImageName = "";
            BaseImageName = "";
            BulletHitPoints = 0;
            BulletExplosionRange = 1;
            BulletSpeed = 0;
        }


        public TurretLevel(
            int niveau,
            int prixAchat,
            int prixVente,
            float zoneActivation,
            double cadenceTir,
            int nombreCanons,
            double tempsConstruction,
            BulletType projectileLance,
            string representation,
            string representationBase,
            float projectilePointsAttaque,
            float projectileZoneImpact,
            float projectileVitesse)
        {
            this.Level = niveau;
            this.BuyPrice = prixAchat;
            this.SellPrice = prixVente;
            this.Range = zoneActivation;
            this.FireRate = cadenceTir;
            this.NbCanons = nombreCanons;
            this.BuildingTime = tempsConstruction;
            this.Bullet = projectileLance;
            this.CanonImageName = representation;
            this.BaseImageName = representationBase;
            this.BulletHitPoints = projectilePointsAttaque;
            this.BulletExplosionRange = projectileZoneImpact;
            this.BulletSpeed = projectileVitesse;
        }
    }
}

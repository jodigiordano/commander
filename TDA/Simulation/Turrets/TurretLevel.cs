namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;

    class TurretLevel
    {
        public int Level;
        public int BuyPrice;
        public int SellPrice;
        public Cercle Range;
        public double ShootingFrequency;
        public int NbCanons;
        public double BuildingTime;
        public BulletType Bullet;
        public String CanonImageName;
        public String BaseImageName;
        public float BulletHitPoints;
        public Cercle BulletExplosionRange;
        public float BulletSpeed;


        public TurretLevel()
        {
            Level = 1;
            BuyPrice = 0;
            SellPrice = 0;
            Range = new Cercle(Vector3.Zero, 1);
            ShootingFrequency = 1;
            NbCanons = 1;
            BuildingTime = 1;
            Bullet = BulletType.Aucun;
            CanonImageName = "";
            BaseImageName = "";
            BulletHitPoints = 0;
            BulletExplosionRange = new Cercle(Vector3.Zero, 1);
            BulletSpeed = 0;
        }


        public TurretLevel(
            int niveau,
            int prixAchat,
            int prixVente,
            Cercle zoneActivation,
            double cadenceTir,
            int nombreCanons,
            double tempsConstruction,
            BulletType projectileLance,
            String representation,
            String representationBase,
            float projectilePointsAttaque,
            Cercle projectileZoneImpact,
            float projectileVitesse)
        {
            this.Level = niveau;
            this.BuyPrice = prixAchat;
            this.SellPrice = prixVente;
            this.Range = zoneActivation;
            this.ShootingFrequency = cadenceTir;
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

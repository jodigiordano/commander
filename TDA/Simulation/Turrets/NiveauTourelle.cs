namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Physique;
    using Core.Visuel;
    using Microsoft.Xna.Framework;

    class NiveauTourelle
    {
        public int Niveau                       { get; set; }
        public int PrixAchat                    { get; set; }
        public int PrixVente                    { get; set; }
        public Cercle ZoneActivation            { get; set; }
        public double CadenceTir                { get; set; }
        public int NombreCanons                 { get; set; }
        public double TempsConstruction         { get; set; }
        public TypeProjectile ProjectileLance   { get; set; }
        public String Representation            { get; set; }
        public String RepresentationBase        { get; set; }
        public float ProjectilePointsAttaque    { get; set; }
        public Cercle ProjectileZoneImpact      { get; set; }
        public float ProjectileVitesse          { get; set; }

        public NiveauTourelle()
        {
            Niveau = 1;
            PrixAchat = 0;
            PrixVente = 0;
            ZoneActivation = new Cercle(Vector3.Zero, 1);
            CadenceTir = 1;
            NombreCanons = 1;
            TempsConstruction = 1;
            ProjectileLance = TypeProjectile.Aucun;
            Representation = "";
            RepresentationBase = "";
            ProjectilePointsAttaque = 0;
            ProjectileZoneImpact = new Cercle(Vector3.Zero, 1);
            ProjectileVitesse = 0;
        }

        public NiveauTourelle(
            int niveau,
            int prixAchat,
            int prixVente,
            Cercle zoneActivation,
            double cadenceTir,
            int nombreCanons,
            double tempsConstruction,
            TypeProjectile projectileLance,
            String representation,
            String representationBase,
            float projectilePointsAttaque,
            Cercle projectileZoneImpact,
            float projectileVitesse)
        {
            this.Niveau = niveau;
            this.PrixAchat = prixAchat;
            this.PrixVente = prixVente;
            this.ZoneActivation = zoneActivation;
            this.CadenceTir = cadenceTir;
            this.NombreCanons = nombreCanons;
            this.TempsConstruction = tempsConstruction;
            this.ProjectileLance = projectileLance;
            this.Representation = representation;
            this.RepresentationBase = representationBase;
            this.ProjectilePointsAttaque = projectilePointsAttaque;
            this.ProjectileZoneImpact = projectileZoneImpact;
            this.ProjectileVitesse = projectileVitesse;
        }
    }
}

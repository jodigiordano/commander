using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Visuel
{
    public class FollowingCamera : Camera
    {
        public virtual IVisible ObjetSuivi          { get; set; }
        public Vector3 DistanceMaximale     { get; set; }
        public virtual Vector3 Vitesse              { get; set; }
        private static double TargetGameTime = (1.0 / 60) * 1000;


        public FollowingCamera(
            IVisible objetSuivi,
            Vector3 positionInitiale,
            Vector3 distanceMaximale,
            Vector3 vitesse,
            bool manuelle,
            Vector2 origine,
            Camera ancienneCamera)
            : base(ancienneCamera)
        {
            this.ObjetSuivi = objetSuivi;
            this.DistanceMaximale = distanceMaximale;
            this.Vitesse = vitesse;
            this.Origine = origine;
            this.Manuelle = true;
            this.Position = positionInitiale;
            this.Manuelle = manuelle;
        }


        public FollowingCamera(
            FollowingCamera ancienneCamera,
            Vector3 positionInitiale,
            Vector3 distanceMaximale,
            Vector3 vitesse,
            bool manuelle,
            Vector2 origine)
            : base(ancienneCamera)
        {
            this.ObjetSuivi = ancienneCamera.ObjetSuivi;
            this.DistanceMaximale = distanceMaximale;
            this.Vitesse = vitesse;
            this.Origine = origine;
            this.Manuelle = true;
            this.Position = positionInitiale;
            this.Manuelle = manuelle;
        }


        public override void Update(GameTime gameTime)
        {
            if (Manuelle)
                deplacementManuel(gameTime);
            else
                deplacementAutomatique(gameTime);
        }


        protected virtual void deplacementManuel(GameTime gameTime)
        {
            this.Position = new Vector3(this.ObjetSuivi.position.X, this.ObjetSuivi.position.Y, this.Position.Z);
        }


        private void deplacementAutomatique(GameTime gameTime)
        {
            Vector3 posCam = Position;
            Vector3 deplacement = new Vector3(
                this.ObjetSuivi.position.X - posCam.X,
                this.ObjetSuivi.position.Y - posCam.Y,
                this.ObjetSuivi.position.Z - posCam.Z);
            deplacement.Normalize();

            float multiplicateur = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / TargetGameTime);
            deplacement.X = deplacement.X * Vitesse.X * multiplicateur;
            deplacement.Y = deplacement.Y * Vitesse.Y * multiplicateur;
            deplacement.Z = deplacement.Z * Vitesse.Z * multiplicateur;

            Vector3 distanceTotale = new Vector3(
                this.ObjetSuivi.position.X - posCam.X,
                this.ObjetSuivi.position.Y - posCam.Y,
                this.ObjetSuivi.position.Z - posCam.Z);

            deplacement.X = (distanceTotale.X < 0) ?
                Math.Max(deplacement.X, distanceTotale.X) :
                Math.Min(deplacement.X, distanceTotale.X);

            deplacement.Y = (distanceTotale.Y < 0) ?
                Math.Max(deplacement.Y, distanceTotale.Y) :
                Math.Min(deplacement.Y, distanceTotale.Y);

            deplacement.Z = 0;

            this.Manuelle = true;
            this.Position = new Vector3(posCam.X + deplacement.X, posCam.Y + deplacement.Y, posCam.Z + deplacement.Z);
            this.Manuelle = false;
        }
    }
}
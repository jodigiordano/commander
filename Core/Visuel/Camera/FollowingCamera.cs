//=============================================================================
//
// Caméra qui suit un objet
//
//=============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Visuel
{
    public class FollowingCamera : Camera
    {
        //=============================================================================
        // Getters / Setters
        //=============================================================================

        /// <summary>
        /// Objet suivi
        /// null = aucun objet suivi
        /// </summary>
        public virtual IVisible ObjetSuivi          { get; set; }

        /// <summary>
        /// Distance maximale entre la caméra et l'objet
        /// </summary>
        public Vector3 DistanceMaximale     { get; set; }

        /// <summary>
        /// Vitesse de déplacement vers la cible
        /// Spécifié en termes vectoriels au lieu d'un trajet
        /// </summary>
        public virtual Vector3 Vitesse              { get; set; }

        /// <summary>
        /// Temps de target entre deux ticks
        /// </summary>
        private static double TargetGameTime = (1.0 / 60) * 1000;


        //=============================================================================
        // Constructeurs
        //=============================================================================

        /// <summary>
        /// Constructeur pour une nouvelle caméra qui suit un objet
        /// </summary>
        /// <param name="objetSuivi">Objet suivi</param>
        /// <param name="positionInitiale">Position initiale de la caméra</param>
        /// <param name="distanceMaximale">Distance maximale entre la caméra et l'objet. Seulement 2d pour l'instant</param>
        /// <param name="vitesse">Vitesse de la caméra en mode automatique</param>
        /// <param name="manuelle">Mode manuel ou automatique</param>
        /// <param name="origine">Position relative à la Scène qui est considérée comme la position (0,0) de la caméra</param>
        /// <param name="ancienneCamera">Ancienne caméra pour récupérer les objets qui écoutent ses événements. Peut être "null"</param>
        /// <remarks>
        /// Si elle est spécifiée, l'ancienne caméra perd ses listeners.
        /// </remarks>
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

        /// <summary>
        /// Constructeur pour une nouvelle caméra qui suit un objet déjà suivi par une autre caméra.
        ///  Utilisé pour switcher d'une caméra à une autre.
        /// </summary>
        /// <param name="ancienneCamera">Caméra qui suivait l'objet.</param>
        /// <param name="positionInitiale">Position initiale de la caméra</param>
        /// <param name="distanceMaximale">Distance maximale entre la caméra et l'objet. Seulement 2d pour l'instant</param>
        /// <param name="vitesse">Vitesse de la caméra en mode automatique</param>
        /// <param name="manuelle">Mode manuel ou automatique</param>
        /// <param name="origine">Position relative à la Scène qui est considérée comme la position (0,0) de la caméra</param>
        /// <remarks>
        /// L'ancienne caméra perd ses listeners.
        /// </remarks>
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


        //=============================================================================
        // Services
        //=============================================================================

        /// <summary>
        /// Mise-à-jour de la caméra
        /// </summary>
        /// <param name="gameTime">Temps</param>
        public override void Update(GameTime gameTime)
        {
            if (Manuelle)
                deplacementManuel(gameTime.ElapsedGameTime.TotalMilliseconds);
            else
                deplacementAutomatique(gameTime.ElapsedGameTime.TotalMilliseconds);

        }

        /// <summary>
        /// Mise-à-jour de la caméra
        /// </summary>
        /// <param name="tempsRelatif">Temps entre deux ticks</param>
        public override void Update(double tempsRelatif)
        {
            if (Manuelle)
                deplacementManuel(tempsRelatif);
            else
                deplacementAutomatique(tempsRelatif);
        }


        //=============================================================================
        // Helpers
        //=============================================================================

        /// <summary>
        /// Déplacement manuel de la caméra
        /// </summary>
        protected virtual void deplacementManuel(double tempsRelatif)
        {
            this.Position = new Vector3(this.ObjetSuivi.position.X, this.ObjetSuivi.position.Y, this.Position.Z);
        }

        /// <summary>
        /// Déplacement automatique de la caméra
        /// </summary>
        /// <param name="tempsRelatif">Temps entre deux ticks</param>
        private void deplacementAutomatique(double tempsRelatif)
        {
            Vector3 posCam = Position;
            Vector3 deplacement = new Vector3(
                this.ObjetSuivi.position.X - posCam.X,
                this.ObjetSuivi.position.Y - posCam.Y,
                this.ObjetSuivi.position.Z - posCam.Z);
            deplacement.Normalize();

            float multiplicateur = (float) (tempsRelatif / TargetGameTime);
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
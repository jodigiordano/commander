//=============================================================================
//
// Caméra qui suit un objet
//
//=============================================================================


namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

    public class PathFollowingCamera : FollowingCamera
    {
        //=============================================================================
        // Getters / Setters
        //=============================================================================

        /// <summary>
        /// Objet suivi
        /// null = aucun objet suivi
        /// </summary>
        new private IVisible ObjetSuivi          { get; set; }

        /// <summary>
        /// Vitesse de déplacement vers la cible
        /// Spécifié en termes vectoriels au lieu d'un trajet
        /// </summary>
        new private Vector3 Vitesse              { get; set; }

        /// <summary>
        /// Trajet 2D suivi par la caméra
        /// </summary>
        public Trajet2D Trajet                     { get; private set; }

        /// <summary>
        /// Temps total passé sur le trajet
        /// </summary>
        private double TempsTotal                { get; set; }


        //=============================================================================
        // Constructeurs
        //=============================================================================

        /// <summary>
        /// Constructeur pour une nouvelle caméra qui suit un objet
        /// </summary>
        /// <param name="trajet">Trajet suivi par la caméra</param>
        /// <param name="positionZ">Position en Z de la caméra</param>
        /// <param name="origine">Position relative à la Scène qui est considérée comme la position (0,0) de la caméra</param>
        /// <param name="ancienneCamera">Ancienne caméra pour récupérer les objets qui écoutent ses événements. Peut être "null"</param>
        /// <remarks>
        /// Si elle est spécifiée, l'ancienne caméra perd ses listeners.
        /// </remarks>

        public PathFollowingCamera(
            Trajet2D trajet,
            float positionZ,
            Vector2 origine,
            Camera ancienneCamera)
            : base(new IVisible(), new Vector3(0, 0, positionZ), Vector3.Zero, Vector3.Zero, true, origine, ancienneCamera)
        {
            this.Trajet = trajet;
            this.Position = new Vector3(Trajet.positionDepart(), this.Position.Z);
            this.Manuelle = true;
            this.TempsTotal = 0;
        }

        public PathFollowingCamera(PathFollowingCamera ancienneCamera, Trajet2D trajet) :
            base(ancienneCamera, ancienneCamera.Position, ancienneCamera.DistanceMaximale, ancienneCamera.Vitesse, ancienneCamera.Manuelle, ancienneCamera.Origine)
        {
            this.Trajet = trajet;
            this.TempsTotal = TempsTotal;
        }


        //=============================================================================
        // Services
        //=============================================================================

        /// <summary>
        /// La destination est atteinte
        /// </summary>
        public bool DestinationAtteinte
        {
            get
            {
                return this.Trajet.position(TempsTotal) == this.Trajet.positionFin();
            }
        }


        //=============================================================================
        // Helpers
        //=============================================================================

        /// <summary>
        /// Déplacement manuel de la caméra
        /// </summary>
        protected override void deplacementManuel(double tempsRelatif)
        {
            this.TempsTotal += tempsRelatif;

            Vector2 posTrajet = Trajet.position(this.TempsTotal);
            this.Position = new Vector3(posTrajet.X, posTrajet.Y, this.Position.Z);
        }
    }
}
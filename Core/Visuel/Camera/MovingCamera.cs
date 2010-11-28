//=============================================================================
//
// Cam�ra qui se d�placement constamment dans une direction
// Note: Pas de mode manuel
//
//=============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Visuel
{
    public class MovingCamera : Camera
    {
        //=============================================================================
        // Getters / Setters
        //=============================================================================

        /// <summary>
        /// Vitesse de d�placement
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
        /// Constructeur pour une nouvelle cam�ra qui suit un objet
        /// </summary>
        /// <param name="objetSuivi">Objet suivi</param>
        /// <param name="positionInitiale">Position initiale de la cam�ra</param>
        /// <param name="distanceMaximale">Distance maximale entre la cam�ra et l'objet. Seulement 2d pour l'instant</param>
        /// <param name="vitesse">Vitesse de la cam�ra en mode automatique</param>
        /// <param name="manuelle">Mode manuel ou automatique</param>
        /// <param name="origine">Position relative � la Sc�ne qui est consid�r�e comme la position (0,0) de la cam�ra</param>
        /// <param name="ancienneCamera">Ancienne cam�ra pour r�cup�rer les objets qui �coutent ses �v�nements. Peut �tre "null"</param>
        /// <remarks>
        /// Si elle est sp�cifi�e, l'ancienne cam�ra perd ses listeners.
        /// </remarks>
        public MovingCamera(
            Vector3 positionInitiale,
            Vector3 vitesse,
            Vector2 origine,
            Camera ancienneCamera)
            : base(ancienneCamera)
        {
            this.Vitesse = vitesse;
            this.Origine = origine;
            this.Manuelle = true;
            this.Position = positionInitiale;
        }


        /// <summary>
        /// Constructeur pour une nouvelle cam�ra qui suit un objet d�j� suivi par une autre cam�ra.
        ///  Utilis� pour switcher d'une cam�ra � une autre.
        /// </summary>
        /// <param name="ancienneCamera">Cam�ra qui suivait l'objet.</param>
        /// <param name="positionInitiale">Position initiale de la cam�ra</param>
        /// <param name="distanceMaximale">Distance maximale entre la cam�ra et l'objet. Seulement 2d pour l'instant</param>
        /// <param name="vitesse">Vitesse de la cam�ra en mode automatique</param>
        /// <param name="manuelle">Mode manuel ou automatique</param>
        /// <param name="origine">Position relative � la Sc�ne qui est consid�r�e comme la position (0,0) de la cam�ra</param>
        /// <remarks>
        /// L'ancienne cam�ra perd ses listeners.
        /// </remarks>
        public MovingCamera(
            FollowingCamera ancienneCamera,
            Vector3 positionInitiale,
            Vector3 vitesse,
            Vector2 origine)
            : base(ancienneCamera)
        {
            this.Vitesse = vitesse;
            this.Origine = origine;
            this.Manuelle = true;
            this.Position = positionInitiale;
        }


        //=============================================================================
        // Services
        //=============================================================================

        /// <summary>
        /// Mise-�-jour de la cam�ra
        /// </summary>
        /// <param name="gameTime">Temps</param>
        public override void Update(GameTime gameTime)
        {
            Update(gameTime.ElapsedGameTime.TotalMilliseconds);
        }


        /// <summary>
        /// Mise-�-jour de la cam�ra
        /// </summary>
        /// <param name="tempsRelatif">Temps entre deux ticks</param>
        public override void Update(double tempsRelatif)
        {
            float multiplicateur = (float)(tempsRelatif / TargetGameTime);

            Vector3 deplacement = new Vector3(
                Vitesse.X * multiplicateur,
                Vitesse.Y * multiplicateur,
                Vitesse.Z * multiplicateur);

            this.Manuelle = true;

            Vector3 pos = Position;
            this.Position = new Vector3(pos.X + deplacement.X, pos.Y + deplacement.Y, pos.Z + deplacement.Z);
            this.Manuelle = false;
        }
    }
}
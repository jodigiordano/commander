//=====================================================================
//
// Contrôle la vitesse de l'objet dans le temps à l'aide d'un trajet 2D
//
//=====================================================================

namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    public class EffetVitesseTrajet : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Trajet2D Trajet          { get; set; }
        public float VitesseDepart      { get; set; }
        public float VitesseArrivee     { get; set; }
        private float deltaVitesse      { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void LogiqueLineaire()
        {
            Objet.Vitesse = VitesseDepart + deltaVitesse * Trajet.position(tempsRelatif).Y;
        }

        protected override void LogiqueApresDuree()
        {
            LogiqueLineaire();
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Vitesse = VitesseArrivee;
        }

        protected override void InitLogique()
        {
            deltaVitesse = VitesseArrivee - VitesseDepart;

            Objet.Vitesse = VitesseDepart;
        }

        protected override void LogiqueTermine()
        {
            Objet.Vitesse = VitesseArrivee;
        }
    }
}

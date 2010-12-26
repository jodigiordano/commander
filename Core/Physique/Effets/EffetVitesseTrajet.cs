//=====================================================================
//
// Contrôle la vitesse de l'objet dans le temps à l'aide d'un trajet 2D
//
//=====================================================================

namespace EphemereGames.Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Utilities;

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

        protected override void LogicLinear()
        {
            Objet.Vitesse = VitesseDepart + deltaVitesse * Trajet.position(ElaspedTime).Y;
        }

        protected override void LogicAfter()
        {
            LogicLinear();
        }

        protected override void LogicNow()
        {
            Objet.Vitesse = VitesseArrivee;
        }

        protected override void InitializeLogic()
        {
            deltaVitesse = VitesseArrivee - VitesseDepart;

            Objet.Vitesse = VitesseDepart;
        }

        protected override void LogicEnd()
        {
            Objet.Vitesse = VitesseArrivee;
        }
    }
}

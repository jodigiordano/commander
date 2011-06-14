//=====================================================================
//
// Contrôle la vitesse de l'objet dans le temps à l'aide d'un trajet 2D
//
//=====================================================================

namespace EphemereGames.Core.Physics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Utilities;

    public class SpeedPathEffect : PhysicalEffect
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Path2D Trajet          { get; set; }
        public float VitesseDepart      { get; set; }
        public float VitesseArrivee     { get; set; }
        private float deltaVitesse      { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void LogicLinear()
        {
            Objet.Speed = VitesseDepart + deltaVitesse * Trajet.position(ElaspedTime).Y;
        }

        protected override void LogicAfter()
        {
            LogicLinear();
        }

        protected override void LogicNow()
        {
            Objet.Speed = VitesseArrivee;
        }

        protected override void InitializeLogic()
        {
            deltaVitesse = VitesseArrivee - VitesseDepart;

            Objet.Speed = VitesseDepart;
        }

        protected override void LogicEnd()
        {
            Objet.Speed = VitesseArrivee;
        }
    }
}

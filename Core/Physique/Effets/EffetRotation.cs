//=====================================================================
//
// Déplacement linéairement un objet de sa position actuelle à la
// position de fin.
//
//=====================================================================

namespace EphemereGames.Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    public class EffetRotation : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public float Quantite;

        private float QuantiteEmise;
        private float QuantiteParTick;


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitializeLogic()
        {
            QuantiteParTick = (float)(Quantite / Length);
        }

        protected override void LogicLinear()
        {
            Objet.Rotation += (float)(TimeOneTick * QuantiteParTick);
            QuantiteEmise -= (float)(TimeOneTick * QuantiteParTick);
        }

        protected override void LogicAfter()
        {
            Objet.Rotation += (Quantite - QuantiteEmise);
        }

        protected override void LogicNow()
        {
            Objet.Rotation += Quantite;
        }
    }
}

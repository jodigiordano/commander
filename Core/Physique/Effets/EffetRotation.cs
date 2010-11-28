//=====================================================================
//
// Déplacement linéairement un objet de sa position actuelle à la
// position de fin.
//
//=====================================================================

namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

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

        protected override void InitLogique()
        {
            QuantiteParTick = (float)(Quantite / Duree);
        }

        protected override void LogiqueLineaire()
        {
            Objet.Rotation += (float)(tempsUnTick * QuantiteParTick);
            QuantiteEmise -= (float)(tempsUnTick * QuantiteParTick);
        }

        protected override void LogiqueApresDuree()
        {
            Objet.Rotation += (Quantite - QuantiteEmise);
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Rotation += Quantite;
        }
    }
}

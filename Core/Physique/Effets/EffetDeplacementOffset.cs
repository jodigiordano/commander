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

    public class EffetDeplacementOffset : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Vector3 Offset { get; set; }

        private Vector3 dernierDeplacement;


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {
            dernierDeplacement = Vector3.Zero;
        }

        protected override void LogiqueLineaire()
        {

            Vector3 nouveauDeplacement = Offset * (float)(tempsRelatif / Duree);

            Objet.Position -= dernierDeplacement;
            Objet.Position += nouveauDeplacement;
            dernierDeplacement = nouveauDeplacement;
        }

        protected override void LogiqueApresDuree()
        {
            Objet.Position += Offset;
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Position += Offset;
        }

        protected override void LogiqueTermine()
        {
            base.LogiqueTermine();

            Objet.Position -= dernierDeplacement;
            Objet.Position += Offset;
            dernierDeplacement = Offset;
        }
    }
}

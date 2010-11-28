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

    public class EffetSuivre : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public IObjetPhysique ObjetSuivi    { get; set; }
        public float Vitesse                { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {

        }

        protected override void LogiqueLineaire()
        {
            Vector3 direction = ObjetSuivi.Position - Objet.Position;
            direction.Normalize();

            Objet.Position += direction * Vitesse;
        }

        protected override void LogiqueApresDuree()
        {
            throw new Exception("TODO");
        }

        protected override void LogiqueMaintenant()
        {
            throw new Exception("TODO");
        }
    }
}

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

    public class EffetDeplacement : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Vector3 PositionFin { get; set; }
        private Vector3 PositionDebut { get; set; }

        private Vector3 deplacement = Vector3.Zero;


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {
            PositionDebut = Objet.Position;

            deplacement = PositionFin - PositionDebut;
        }

        protected override void LogiqueLineaire()
        {
            Objet.Position =
                new Vector3(
                    (float)(PositionDebut.X + (deplacement.X * (tempsRelatif / Duree))),
                    (float)(PositionDebut.Y + (deplacement.Y * (tempsRelatif / Duree))),
                    (float)(PositionDebut.Z + (deplacement.Z * (tempsRelatif / Duree))));
        }

        protected override void LogiqueApresDuree()
        {
            Objet.Position = PositionFin;
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Position = PositionFin;
        }
    }
}

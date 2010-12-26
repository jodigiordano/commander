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

        protected override void InitializeLogic()
        {
            PositionDebut = Objet.Position;

            deplacement = PositionFin - PositionDebut;
        }

        protected override void LogicLinear()
        {
            Objet.Position =
                new Vector3(
                    (float)(PositionDebut.X + (deplacement.X * (ElaspedTime / Length))),
                    (float)(PositionDebut.Y + (deplacement.Y * (ElaspedTime / Length))),
                    (float)(PositionDebut.Z + (deplacement.Z * (ElaspedTime / Length))));
        }

        protected override void LogicAfter()
        {
            Objet.Position = PositionFin;
        }

        protected override void LogicNow()
        {
            Objet.Position = PositionFin;
        }
    }
}

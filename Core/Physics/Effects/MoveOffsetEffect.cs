//=====================================================================
//
// Déplacement linéairement un objet de sa position actuelle à la
// position de fin.
//
//=====================================================================

namespace EphemereGames.Core.Physics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    public class MoveOffsetEffect : PhysicalEffect
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Vector3 Offset { get; set; }

        private Vector3 dernierDeplacement;


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitializeLogic()
        {
            dernierDeplacement = Vector3.Zero;
        }

        protected override void LogicLinear()
        {

            Vector3 nouveauDeplacement = Offset * (float)(ElaspedTime / Length);

            Objet.Position -= dernierDeplacement;
            Objet.Position += nouveauDeplacement;
            dernierDeplacement = nouveauDeplacement;
        }

        protected override void LogicAfter()
        {
            Objet.Position += Offset;
        }

        protected override void LogicNow()
        {
            Objet.Position += Offset;
        }

        protected override void LogicEnd()
        {
            base.LogicEnd();

            Objet.Position -= dernierDeplacement;
            Objet.Position += Offset;
            dernierDeplacement = Offset;
        }
    }
}

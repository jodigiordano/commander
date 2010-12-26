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

        protected override void InitializeLogic()
        {

        }

        protected override void LogicLinear()
        {
            Vector3 direction = ObjetSuivi.Position - Objet.Position;
            direction.Normalize();

            Objet.Position += direction * Vitesse;
        }

        protected override void LogicAfter()
        {
            throw new Exception("TODO");
        }

        protected override void LogicNow()
        {
            throw new Exception("TODO");
        }
    }
}

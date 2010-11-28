//=====================================================================
//
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class EffetDessinerPartie : EffetVisuel, ICloneable
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public bool DessinerPartie { get; set; }
        public Rectangle PartieVisible { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique() {}

        protected override void LogiqueLineaire()
        {
            throw new Exception("Pas logique!");
        }

        protected override void LogiqueApresDuree()
        {
            Objet.DessinerPartie = DessinerPartie;
            Objet.partieVisible = PartieVisible;
        }

        protected override void LogiqueMaintenant()
        {
            Objet.DessinerPartie = DessinerPartie;
            Objet.partieVisible = PartieVisible;
        }


        #region ICloneable Members

        object ICloneable.Clone()
        {
            EffetDessinerPartie edp = (EffetDessinerPartie)base.Clone();
            edp.DessinerPartie = this.DessinerPartie;
            edp.PartieVisible = this.PartieVisible;

            return edp;
        }

        #endregion
    }
}

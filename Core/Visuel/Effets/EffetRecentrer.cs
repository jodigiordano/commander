//=====================================================================
//
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    
    public class EffetRecentrer : EffetVisuel
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Vector2 OrigineFin { get; set; }
        private Vector2 origineDebut;
        private Vector2 deplacement;


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {
            origineDebut = Objet.Origine;
            deplacement = OrigineFin - origineDebut;
        }

        protected override void LogiqueLineaire()
        {
            Vector2 nouvelleOrigine = new Vector2();
            nouvelleOrigine.X = (float)(origineDebut.X + (deplacement.X * (tempsRelatif / Duree)));
            nouvelleOrigine.Y = (float)(origineDebut.Y + (deplacement.Y * (tempsRelatif / Duree)));

            Objet.Origine = nouvelleOrigine;
        }

        protected override void LogiqueApresDuree()
        {
            Objet.Origine = OrigineFin;
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Origine = OrigineFin;
        }
    }
}

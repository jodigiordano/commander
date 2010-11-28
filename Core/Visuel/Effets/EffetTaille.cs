//=====================================================================
//
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;

    public class EffetTaille : EffetVisuel
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Trajet2D Trajet { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique() {}

        protected override void LogiqueLineaire()
        {
            Objet.TailleVecteur = Trajet.position(tempsRelatif);
        }

        protected override void LogiqueApresDuree()
        {
            Objet.TailleVecteur = Trajet.position(tempsRelatif);
        }

        protected override void LogiqueMaintenant()
        {
            Objet.TailleVecteur = Trajet.position(Duree);
        }
    }
}

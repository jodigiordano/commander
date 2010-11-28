//=====================================================================
//
// TODO : utilise un trajet et non le Fade/FadeCouleur habituel
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    public class EffetFadeCouleur : EffetVisuel
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Trajet2D Trajet { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {
            Objet.Couleur.A = (byte)(Trajet.positionDepart().Y * 255);
        }

        protected override void LogiqueLineaire()
        {
            Objet.Couleur.A = (byte) (Trajet.position(tempsRelatif).Y * 255);
        }

        protected override void LogiqueApresDuree()
        {
            Objet.Couleur.A = (byte)(Trajet.position(Duree).Y * 255);
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Couleur.A = (byte)(Trajet.position(Duree).Y * 255);
        }
    }
}

//=====================================================================
//
//
//
//=====================================================================

namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Utilities;

    public class EffetDeplacementTrajet : EffetPhysique
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Trajet2D Trajet { private get; set; }
        public float Rotation { get; private set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void LogiqueLineaire()
        {
            Objet.Position = new Vector3(Trajet.position(tempsRelatif), Objet.Position.Z);
            Rotation = Trajet.rotation(tempsRelatif);
        }

        protected override void LogiqueApresDuree()
        {
            LogiqueLineaire();
        }

        protected override void LogiqueMaintenant()
        {
            Objet.Position = new Vector3(Trajet.position(Duree), Objet.Position.Z);
            Rotation = Trajet.rotation(Duree);
        }

        protected override void InitLogique()
        {
            Objet.Position = new Vector3(Trajet.positionDepart(), Objet.Position.Z);
            Rotation = Trajet.rotation(0);
        }
    }
}

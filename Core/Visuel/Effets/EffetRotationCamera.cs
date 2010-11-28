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

    public class EffetRotationCamera : EffetVisuel
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Trajet2D VitesseRotation;
        public float Rotation;
        private bool ManuelleBackup;


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {
            Scene scene = (Scene)Objet;

            ManuelleBackup = scene.Camera.Manuelle;
            scene.Camera.Manuelle = (this.Progression == TypeProgression.Lineaire) ? false : true;
            scene.Camera.Rotation               = Rotation;
            scene.Camera.VitesseRotation        = (VitesseRotation == null) ? Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, this.Duree) : VitesseRotation;
        }

        protected override void LogiqueLineaire()
        {
            Scene scene = (Scene)Objet;

            scene.Camera.Update(tempsRelatif);
        }

        protected override void LogiqueApresDuree() {}

        protected override void LogiqueMaintenant() {}

        protected override void LogiqueTermine()
        {
            Scene scene = (Scene)Objet;

            scene.Camera.Manuelle = ManuelleBackup;
        }
    }
}

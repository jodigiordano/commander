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

    public class EffetCamera : EffetVisuel, ICloneable
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        [ContentSerializer(Optional = true)]
        public Trajet2D VitesseDeplacement        { get; set; }

        [ContentSerializer(Optional = true)]
        public Trajet2D VitesseRotation { get; set; }

        [ContentSerializer(Optional = true)]
        public Trajet2D VitesseZoom { get; set; }

        public Vector3 Position                 { get; set; }
        public float Rotation                   { get; set; }


        //=====================================================================
        // Logique
        //=====================================================================

        protected override void InitLogique()
        {
            Scene scene = (Scene)Objet;

            scene.Camera.Manuelle               = (this.Progression == TypeProgression.Lineaire) ? false : true;
            scene.Camera.Position               = Position;
            scene.Camera.Rotation               = Rotation;
            scene.Camera.VitesseDeplacement     = (VitesseDeplacement == null) ? Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, this.Duree) : VitesseDeplacement;
            scene.Camera.VitesseRotation        = (VitesseRotation == null) ? Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, this.Duree) : VitesseRotation;
            scene.Camera.VitesseZoom            = (VitesseZoom == null) ? Trajet2D.CreerVitesse(Trajet2D.Type.Lineaire, this.Duree) : VitesseZoom;
        }

        protected override void LogiqueLineaire()
        {
            Scene scene = (Scene)Objet;

            scene.Camera.Update(tempsRelatif);
        }

        protected override void LogiqueApresDuree() {}

        protected override void LogiqueMaintenant() {}

        #region ICloneable Members

        public override object Clone()
        {
            EffetCamera ec = (EffetCamera) base.Clone();
            ec.Rotation = this.Rotation;
            ec.Termine = false;
            ec.VitesseDeplacement = this.VitesseDeplacement;
            ec.VitesseRotation = this.VitesseRotation;
            ec.VitesseZoom = this.VitesseZoom;

            return ec;
        }

        #endregion
    }
}

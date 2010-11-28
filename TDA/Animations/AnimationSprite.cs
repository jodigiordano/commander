namespace TDA
{
    //=========================================================================
    #region Importations
    //=========================================================================
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;
    #endregion
    
    class AnimationSprite : Animation
    {

        //=====================================================================
        #region Attributs
        //=====================================================================

        //private Sprite sprite;
        private double tempsPasse = 0;  // temps passé depuis le dernier appel de sprite.suivant()
        #endregion

        //=====================================================================
        #region Constructeur
        //=====================================================================

        public AnimationSprite(Sprite sprite)
            : base (sprite.TempsDefilement)
        {
            this.Objet = sprite;
            //this.sprite = sprite;
            this.Melange = sprite.Melange;
        }
        #endregion

        //=====================================================================
        #region Services
        //=====================================================================

        public override void suivant(GameTime gameTime)
        {
            if (tempsPasse >= ((Sprite)Objet).VitesseDefilement)
            {
                ((Sprite)Objet).suivant();
                tempsPasse = 0;
            }
            
            tempsPasse += gameTime.ElapsedGameTime.TotalMilliseconds;

            base.suivant(gameTime);
        }
        #endregion
    }
}

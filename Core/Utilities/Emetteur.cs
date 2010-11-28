//=====================================================================
//
//
//=====================================================================

namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public class Emetteur
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        private double intervalEmission;        // Temps entre deux émissions
        private double derniereEmission = 0;    // Depuis quand a eu lieu la dernière émission
        private double tempsEmission;
        private double tempsRestant;


        //=====================================================================
        // Constructeur
        //=====================================================================

        public Emetteur(double intervalEmission, double tempsEmission)
        {
            this.intervalEmission = intervalEmission;
            this.tempsEmission = tempsEmission;
            tempsRestant = this.tempsEmission;
        }


        //=====================================================================
        // Logique
        //=====================================================================

        public bool peutEmettre(GameTime gameTime)
        {
            if (derniereEmission >= intervalEmission && intervalEmission <= tempsRestant)
            {
                derniereEmission = 0;

                Update(gameTime);
                return true;
            }

            Update(gameTime);
            return false;
        }


        private void Update(GameTime gameTime)
        {
            derniereEmission += gameTime.ElapsedGameTime.TotalMilliseconds;
            tempsRestant -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}

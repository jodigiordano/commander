//=====================================================================
//
// Pause un objet
//
// Cet objet garde le temps total passé en pause. Un premier appel à
// togglePause() met l'objet en pause. Le temps écoulé avant
// un deuxième appel à cette méthode est enregistré.
//
// On peut ensuite faire un appel à gameTimeAjuste() qui
// retourne le temps actuel moins le temps passé en pause.
//
//=====================================================================

namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public class Pause
    {
        //=====================================================================
        // Attributs
        //=====================================================================

        private TimeSpan tempsDebutPauseGame = new TimeSpan();   // temps du début de la pause (game time)
        private TimeSpan tempsDebutPauseReal = new TimeSpan();   // temps du début de la pause (real time)
        private TimeSpan tempsTotalPauseGame = new TimeSpan();   // temps total passé en pause (game time)
        private TimeSpan tempsTotalPauseReal = new TimeSpan();   // temps total passé en payse (real time)


        //=====================================================================
        // Getters / Setters
        //=====================================================================

        private bool EnPause { get; set; }


        //=====================================================================
        // Constructeur
        //=====================================================================

        public Pause()
        {
            this.EnPause = false;
        }


        //=====================================================================
        // Logique
        //=====================================================================

        //
        // Met en pause et vice-versa
        //

        public void pauser(GameTime gameTime)
        {
            if (EnPause)
                return;

            tempsDebutPauseGame = gameTime.TotalGameTime;
            tempsDebutPauseReal = gameTime.TotalRealTime;

            EnPause = true;
        }

        public void depauser(GameTime gameTime)
        {
            if (!EnPause)
                return;

            tempsTotalPauseGame += gameTime.TotalGameTime - tempsDebutPauseGame;
            tempsTotalPauseReal += gameTime.TotalRealTime - tempsDebutPauseReal;

            tempsDebutPauseGame = new TimeSpan();
            tempsDebutPauseReal = new TimeSpan();

            EnPause = false;
        }


        //
        // Retourne le GameTime ajusté en tenant compte du temps total
        // passé en pause
        //

        public GameTime gameTimeAjuste(GameTime gameTime)
        {
            return new GameTime
            (
                EnPause ? (gameTime.TotalRealTime - tempsTotalPauseReal) - (gameTime.TotalRealTime - tempsDebutPauseReal) : gameTime.TotalRealTime - tempsTotalPauseReal,
                EnPause ? new TimeSpan() : gameTime.ElapsedRealTime,
                EnPause ? (gameTime.TotalGameTime - tempsTotalPauseGame) - (gameTime.TotalGameTime - tempsDebutPauseGame) : gameTime.TotalGameTime - tempsTotalPauseGame,
                EnPause ? new TimeSpan() : gameTime.ElapsedGameTime
            );
        }
    }
}

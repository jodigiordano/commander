namespace EphemereGames.Commander
{
    //=========================================================================
    #region Importations
    //=========================================================================

    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Physique;

    #endregion


    /// <summary>
    /// Contrôle les projectiles sur le terrain de jeu.
    /// Il est responsable de leur mise à jour et de leur création/destruction.
    /// </summary>
    class ControleurProjectiles : DrawableGameComponent
    {
        //=====================================================================
        #region Attributs
        //=====================================================================

        /// <summary>
        /// Scène dans laquelle le contrôleur est utilisé
        /// </summary>
        private Scene Scene;

        /// <summary>
        /// Liste des projectiles sur le terrain de jeu
        /// </summary>
        public List<Projectile> Projectiles { get; private set; }

        /// <summary>
        /// Obtient le nombre de projectiles
        /// </summary>
        public int NbProjectiles { get { return Projectiles.Count; } }

        #endregion


        //=====================================================================
        #region Initialisation
        //=====================================================================

        /// <summary>
        /// Construit un nouveau contrôleur de projectiles
        /// </summary>
        /// <param name="main">Référence sur l'objet «Xna.Game» dont fait parti ce composant</param>
        /// <param name="partie">Référence sur la «Scene» dont fait parti ce composant</param>
        public ControleurProjectiles(Simulation simulation)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;

            Projectiles = new List<Projectile>();
        }

        #endregion


        //=====================================================================
        #region Services
        //=====================================================================

        /// <summary>
        /// Met à jour la logique u temps de jeu.
        /// </summary>
        /// <param name="gameTime">Le temps de jeu</param>
        public override void Update(GameTime gameTime)
        {
            // Pour les projectiles qui meurent par eux-memes
            for (int i = Projectiles.Count - 1; i > -1; i--)
                if (!Projectiles[i].EstVivant)
                {
                    Projectiles[i].doMeurt();
                    Projectiles.RemoveAt(i);
                }

            for (int i = 0; i < Projectiles.Count; i++)
                Projectiles[i].Update(gameTime);
        }


        /// <summary>
        /// Met à jour le visuel du temps de jeu
        /// </summary>
        /// <param name="gameTime">Le temps de jeu</param>
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Projectiles.Count; i++)
                Projectiles[i].Draw(null);
        }

        #endregion


        //=====================================================================
        #region Événements
        //=====================================================================

        /// <summary>
        /// Événement lancé lorsqu'un projectile est détruit
        /// </summary>
        public event PhysicalObjectHandler ObjetDetruit;


        /// <summary>
        /// Action effectuée lorsqu'un objet déserte le terrain de jeu
        /// </summary>
        /// <param name="deserteur">Référence sur l'objet déserteur</param>
        //public void doObjetDeserte(IObjetPhysique deserteur)
        //{
        //    Projectile projectile = deserteur as Projectile;

        //    if (projectile == null)
        //        return;

        //    projectile.doMeurtSilencieusement();

        //    Projectiles.Remove(projectile);
        //}


        /// <summary>
        /// Action effectuée lorsqu'un objet est touché par un autre objet
        /// </summary>
        /// <param name="objet">L'objet touché</param>
        /// <param name="par">L'objet qui touche</param>
        //public void doObjetTouche(IObjetPhysique objet, IObjetPhysique par)
        //{
        //    Ennemi ennemi = par as Ennemi;
        //    Projectile projectile = objet as Projectile;

        //    if (ennemi == null || projectile == null)
        //        return;

        //    projectile.doTouche(ennemi);

        //    if (!projectile.EstVivant)
        //    {
        //        projectile.doMeurt();
        //        Projectiles.Remove(projectile);

        //        notifyObjetDetruit(projectile);
        //    }
        //}


        /// <summary>
        /// Action effectuée lorsqu'un nouvel objet est créé sur le terrain de jeu
        /// </summary>
        /// <param name="objet">L'objet nouvellement créé</param>
        public void doObjetCree(IObjetPhysique objet)
        {
            if (!(objet is Projectile))
                return;

            Projectiles.Add((Projectile) objet);
        }


        /// <summary>
        /// Notifie les abonnés qu'un projectile a été détruit
        /// </summary>
        /// <param name="projectile">Le projectile détruit</param>
        protected void notifyObjetDetruit(Projectile projectile)
        {
            if (ObjetDetruit != null)
                ObjetDetruit(projectile);
        }

        #endregion
    }
}

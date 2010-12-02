namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;
    using Microsoft.Xna.Framework.GamerServices;


    class ControleurJoueursConnectes
    {
        public List<Joueur> JoueursConnectes;

        public delegate void JoueurPrincipalDeconnecteHandler();
        public event JoueurPrincipalDeconnecteHandler JoueurPrincipalDeconnecte;

        private Main Main;

        public ControleurJoueursConnectes(Main main)
        {
            Main = main;

            JoueursConnectes = new List<Joueur>();

            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(joueurDeconnecte);
        }

        public void Initialize()
        {
            JoueursConnectes.Clear();
            //JoueurPrincipalDeconnecte = null;
        }

        private void joueurDeconnecte(object sender, SignedOutEventArgs e)
        {
            if (JoueursConnectes.Count > 0 && JoueursConnectes[0].Manette == e.Gamer.PlayerIndex)
                notifyJoueurPrincipalDeconnecte();
            else if (JoueursConnectes.Count > 1)
                JoueursConnectes.RemoveAt(1);
        }


        protected virtual void notifyJoueurPrincipalDeconnecte()
        {
            if (JoueurPrincipalDeconnecte != null)
                JoueurPrincipalDeconnecte();
        }
    }
}

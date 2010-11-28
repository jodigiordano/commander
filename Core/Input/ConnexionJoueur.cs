namespace Core.Input
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    
    class ConnexionJoueur
    {
        private static ConnexionJoueur instance = new ConnexionJoueur();

        private ConnexionJoueur() { }

        public static ConnexionJoueur Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Demande au joueur de se connecter (sauf s'il ne l'est déjà).
        /// ATTENTION: Ne pas considérer que le joueur sera connecté une fois la fonction terminée !
        /// </summary>
        /// <param name="joueur">Le joueur à connecter.</param>
        public void  connecter(PlayerIndex joueur)
        {
            if (Guide.IsVisible)
                return;

            // Si aucun profil n'est associé au joueur passé en paramètre, on lui demande de se connecter
            if (SignedInGamer.SignedInGamers[joueur] == null)
                Guide.ShowSignIn(1, false);
        }

        public SignedInGamer getJoueurConnecte(PlayerIndex joueur)
        {
            return SignedInGamer.SignedInGamers[joueur];
        }

#if XBOX
        public int getPreferenceDifficulte(PlayerIndex joueur)
        {
            return (int) getJoueurConnecte(joueur).GameDefaults.GameDifficulty;
        }

        public ControllerSensitivity getPreferenceSensibiliteGamePad(PlayerIndex joueur)
        {
            return getJoueurConnecte(joueur).GameDefaults.ControllerSensitivity;
        }
#endif
    }
}

namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;

    class Joueur
    {
        private SignedInGamer JoueurPhysique;

        private PlayerIndex manette;
        public PlayerIndex Manette
        {
            get
            {
#if WINDOWS
                return manette;
#else
                return JoueurPhysique.PlayerIndex;
#endif
            }

            set
            {
                manette = value;
            }
        }

        public Joueur(PlayerIndex manette)
        {
#if WINDOWS
            this.manette = manette;
#else
            JoueurPhysique = Core.Input.Facade.getJoueurConnecte(manette);
#endif
        }
    }
}
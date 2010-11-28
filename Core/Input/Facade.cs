//=============================================================================
//
// Point d'entrée dans la librairie
//
//=============================================================================

namespace Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.GamerServices;
    
    public static class Facade
    {
#if WINDOWS && !MANETTE_WINDOWS
            private static GestionnaireTouches<Keys> gestionnaireTouches = GestionnaireTouches<Keys>.Instance;
            private static GestionnaireTouches<BoutonSouris> gestionnaireSouris = GestionnaireTouches<Keys>.InstanceSouris;
#else
        private static GestionnaireTouches<Buttons> gestionnaireTouches = GestionnaireTouches<Buttons>.Instance;
#endif

        public static void Initialize(String[] nomsScenes, Vector2 positionBaseSouris)
        {
            foreach (var nomScene in nomsScenes)
                gestionnaireTouches.ajouterScene(nomScene);

#if WINDOWS && !MANETTE_WINDOWS
            foreach (var nomScene in nomsScenes)
                gestionnaireSouris.ajouterScene(nomScene);

            gestionnaireTouches.PositionBaseSouris = positionBaseSouris;
            gestionnaireSouris.PositionBaseSouris = positionBaseSouris;

            Mouse.SetPosition
            (
                (int)(positionBaseSouris.X),
                (int)(positionBaseSouris.Y)
            );
#endif
        }

        public static void connecterJoueur(PlayerIndex joueur)
        {
            ConnexionJoueur.Instance.connecter(joueur);
        }

        public static SignedInGamer getJoueurConnecte(PlayerIndex joueur)
        {
            return ConnexionJoueur.Instance.getJoueurConnecte(joueur);
        }

        public static void Update(GameTime gameTime)
        {
            gestionnaireTouches.Update(gameTime);

#if WINDOWS && !MANETTE_WINDOWS
            gestionnaireSouris.Update(gameTime);

            Mouse.SetPosition
            (
                (int)(gestionnaireSouris.PositionBaseSouris.X),
                (int)(gestionnaireSouris.PositionBaseSouris.Y)
            );
#endif

            Vibrations.Instance.Update(gameTime);
        }

        public static void activerScene(string nomScene)
        {
            gestionnaireTouches.activer(nomScene);
        }

        public static void desactiverScene(string nomScene)
        {
            gestionnaireTouches.desactiver(nomScene);
        }


#if WINDOWS && !MANETTE_WINDOWS
        public static bool estPesee(Keys touche, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireTouches.estPesee(touche, playerIndex, nomScene);
        }

        public static bool estPesee(BoutonSouris touche, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireSouris.estPesee(touche, playerIndex, nomScene);
        }
#else
        public static bool estPesee(Buttons touche, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireTouches.estPesee(touche, playerIndex, nomScene);
        }
#endif

#if WINDOWS && !MANETTE_WINDOWS
        public static bool estPeseeUneSeuleFois(Keys touche, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireTouches.estPeseeUneSeuleFois(touche, playerIndex, nomScene);
        }

        public static bool estPeseeUneSeuleFois(BoutonSouris touche, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireSouris.estPeseeUneSeuleFois(touche, playerIndex, nomScene);
        }
#else
        public static bool estPeseeUneSeuleFois(Buttons touche, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireTouches.estPeseeUneSeuleFois(touche, playerIndex, nomScene);
        }
#endif



#if WINDOWS && !MANETTE_WINDOWS
        public static void ignorerTouches(PlayerIndex playerIndex, List<Keys> touches, string nomScene)
        {
            gestionnaireTouches.SetTouchesIgnorees(playerIndex, touches, nomScene);
        }

        public static void ignorerTouches(PlayerIndex playerIndex, List<BoutonSouris> touches, string nomScene)
        {
            gestionnaireSouris.SetTouchesIgnorees(playerIndex, touches, nomScene);
        }
#else
        public static void ignorerTouches(PlayerIndex playerIndex, List<Buttons> touches, string nomScene)
        {
            gestionnaireTouches.SetTouchesIgnorees(playerIndex, touches, nomScene);
        }
#endif


#if WINDOWS && !MANETTE_WINDOWS
        public static void considerTouches(PlayerIndex playerIndex, List<Keys> touches, string nomScene)
        {
            gestionnaireTouches.SetTouchesTraitees(playerIndex, touches, nomScene);
            //gestionnaireSouris.SetTouchesTraitees(playerIndex, new List<BoutonSouris>(), nomScene);
        }

        public static void considerTouches(PlayerIndex playerIndex, List<BoutonSouris> touches, string nomScene)
        {
            gestionnaireSouris.SetTouchesTraitees(playerIndex, touches, nomScene);
            //gestionnaireTouches.SetTouchesTraitees(playerIndex, new List<Keys>(), nomScene);
        }

        public static void considererSouris(PlayerIndex playerIndex, bool considerer, string nomScene)
        {
            gestionnaireSouris.SetSourisTraitee(playerIndex, considerer, nomScene);
            gestionnaireTouches.SetSourisTraitee(playerIndex, considerer, nomScene);
        }
#else
        public static void considerTouches(PlayerIndex playerIndex, List<Buttons> touches, string nomScene)
        {
            gestionnaireTouches.SetTouchesTraitees(playerIndex, touches, nomScene);
        }
#endif


#if WINDOWS && !MANETTE_WINDOWS
        public static void considerToutesTouches(PlayerIndex playerIndex, string nomScene)
        {
            gestionnaireTouches.SetTouchesTraitees(playerIndex, null, nomScene);
            gestionnaireSouris.SetTouchesTraitees(playerIndex, null, nomScene);
            gestionnaireTouches.SetSourisTraitee(playerIndex, true, nomScene);
            gestionnaireSouris.SetSourisTraitee(playerIndex, true, nomScene);
        }
#else
        public static void considerToutesTouches(PlayerIndex playerIndex, string nomScene)
        {
            gestionnaireTouches.SetTouchesTraitees(playerIndex, null, nomScene);
        }
#endif

#if WINDOWS && !MANETTE_WINDOWS
        public static void considerAucuneTouche(PlayerIndex playerIndex, string nomScene)
        {
            gestionnaireTouches.SetTouchesTraitees(playerIndex, new List<Keys>(), nomScene);
            gestionnaireSouris.SetTouchesTraitees(playerIndex, new List<BoutonSouris>(), nomScene);
        }
#else
        public static void considerAucuneTouche(PlayerIndex playerIndex, string nomScene)
        {
            gestionnaireTouches.SetTouchesTraitees(playerIndex, new List<Buttons>(), nomScene);
        }
#endif


#if XBOX || MANETTE_WINDOWS
        public static void considerThumbsticks(PlayerIndex playerIndex, List<Buttons> touches, string nomScene)
        {
            gestionnaireTouches.SetThumbsticksTraites(playerIndex, touches, nomScene);
        }
#endif


#if WINDOWS && !MANETTE_WINDOWS
        public static void ignorerToucheCeTick(PlayerIndex playerIndex, Keys touche)
        {
            gestionnaireTouches.keyCantBePressedAgainThisTick(touche, playerIndex);
        }

        public static void ignorerToucheCeTick(PlayerIndex playerIndex, BoutonSouris touche)
        {
            gestionnaireSouris.keyCantBePressedAgainThisTick(touche, playerIndex);
        }
#else
        public static void ignorerToucheCeTick(PlayerIndex playerIndex, Buttons touche)
        {
            gestionnaireTouches.keyCantBePressedAgainThisTick(touche, playerIndex);
        }
#endif


        public static Vector2 positionThumbstick(PlayerIndex manette, bool gauche, string nomScene)
        {
            return gestionnaireTouches.positionThumbstick(manette, gauche, GamePadDeadZone.Circular, nomScene);
        }


        public static void vibrerManette(PlayerIndex playerIndex, double temps, float moteurGauche, float moteurDroit)
        {
            Vibrations.Instance.vibrer(playerIndex, temps, moteurGauche, moteurDroit);
        }


        public static void ignorerThumbsticks(PlayerIndex playerIndex, List<Buttons> boutons, string nomScene)
        {
            gestionnaireTouches.SetThumbsticksIgnores(playerIndex, boutons, nomScene);
        }

#if WINDOWS && !MANETTE_WINDOWS
        public static bool estRelachee(int bouton, PlayerIndex playerIndex, string nomScene)
        {
            return (bouton >= 10000) ?
                gestionnaireTouches.estRelachee((Keys)bouton, playerIndex, nomScene) :
                gestionnaireSouris.estRelachee((BoutonSouris)bouton, playerIndex, nomScene);
        }
#else
        public static bool estRelachee(Buttons bouton, PlayerIndex playerIndex, string nomScene)
        {
            return gestionnaireTouches.estRelachee(bouton, playerIndex, nomScene);
        }
#endif


#if XBOX
        public static int getPreferenceDifficulte(PlayerIndex playerIndex)
        {
            return ConnexionJoueur.Instance.getPreferenceDifficulte(playerIndex);
        }

        public static ControllerSensitivity getPreferenceSensibiliteGamePad(PlayerIndex playerIndex)
        {
            return ConnexionJoueur.Instance.getPreferenceSensibiliteGamePad(playerIndex);
        }
#endif

#if WINDOWS && !MANETTE_WINDOWS
        public static Vector2 positionDeltaSouris(PlayerIndex manette, String nomScene)
        {
            return gestionnaireSouris.positionDeltaSouris(manette, nomScene);
        }

        //public static Vector2 positionSouris(PlayerIndex manette, String nomScene)
        //{
        //    return gestionnaireSouris.positionSouris(manette, nomScene);
        //}


        //public static void setDeltaPositionSouris(PlayerIndex manette, String nomScene, Vector2 nouvellePosition)
        //{
        //    gestionnaireSouris.setPositionSouris(manette, nomScene, nouvellePosition);
        //}
        //public static void setPositionSouris(PlayerIndex manette, String nomScene, ref Vector2 nouvellePosition)
        //{
        //    gestionnaireSouris.setPositionSouris(manette, nomScene, ref nouvellePosition);
        //}

        public static int getDeltaRoueSouris(PlayerIndex manette, String nomScene)
        {
            return gestionnaireSouris.getDeltaRoueSouris(manette, nomScene);
        }

#endif
    }
}

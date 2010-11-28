//=====================================================================
//
// Touches traitées et ignorées pour une Scène
//
//=====================================================================

namespace Core.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Core.Visuel;

    public enum BoutonSouris
    {
        Gauche                  = 10000,
        Milieu                  = 10001,
        MilieuHaut              = 10002,
        MilieuBas               = 10003,
        Droite                  = 10004,
        DroiteEtMilieu          = 10005,
        DroiteEtGauche          = 10006,
        MilieuEtGauche          = 10007
    }


    class TouchesScene<Z>
    {
        private String NomScene { get; set; }

        public bool Desactive = false;                                                  //toutes les touches sont ignorées

        public Dictionary<PlayerIndex, List<Z>> TouchesTraitees =
            new Dictionary<PlayerIndex, List<Z>>();                                     //si une liste est null == toutes les touches du PlayerIndex sont traitées
        public Dictionary<PlayerIndex, List<Z>> TouchesIgnorees =
            new Dictionary<PlayerIndex, List<Z>>();                                     //null == toutes les touches sont traitées. Version inverse de touchesTraitees. ATTENTION: aucune synchronisation entre les deux.
        public Dictionary<PlayerIndex, bool> SourisTraitee =
            new Dictionary<PlayerIndex, bool>();                                        //spécifique à Windows
        public Dictionary<PlayerIndex, List<Buttons>> ThumbstickIgnores =
            new Dictionary<PlayerIndex, List<Buttons>>();                               //spécifique à la Xbox
        public Dictionary<PlayerIndex, List<Buttons>> ThumbstickTraites =
            new Dictionary<PlayerIndex, List<Buttons>>();                               //spécifique à la Xbox
        

        public TouchesScene(String nomScene)
        {
            NomScene = nomScene;

            for (PlayerIndex manette = PlayerIndex.One; manette <= PlayerIndex.Four; manette++)
            {
                TouchesTraitees.Add(manette, null);
                TouchesIgnorees.Add(manette, null);
                ThumbstickIgnores.Add(manette, null);
                ThumbstickTraites.Add(manette, null);
                SourisTraitee.Add(manette, true);
            }
        }


        //=====================================================================
        // Logique
        //=====================================================================

        //
        // Est-ce que la touche est présentement pesée
        //

        public bool estPesee(Z key, PlayerIndex indexJoueur)
        {
            if (Desactive)
                return false;

            if (!Core.Visuel.Facade.sceneEnFocus(NomScene))
                return false;

            if (TouchesTraitees[indexJoueur] != null && !TouchesTraitees[indexJoueur].Contains(key))
                return false;

            if (TouchesIgnorees[indexJoueur] != null && TouchesIgnorees[indexJoueur].Contains(key))
                return false;

            object temp = key;

            if (key is Keys)
            {
                return Keyboard.GetState().IsKeyDown((Keys)temp);
            }

            else if (key is Buttons)
            {
                return GamePad.GetState(indexJoueur).IsButtonDown((Buttons)temp);
            }

#if WINDOWS && !MANETTE_WINDOWS
            else if (key is BoutonSouris)
            {
                BoutonSouris bouton = (BoutonSouris) temp;

                switch (bouton)
                {
                    case BoutonSouris.Droite: return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.RightButton == ButtonState.Pressed; break;
                    case BoutonSouris.Gauche: return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.LeftButton == ButtonState.Pressed; break;
                    case BoutonSouris.Milieu: return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.MiddleButton == ButtonState.Pressed; break;
                    case BoutonSouris.DroiteEtGauche: return
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.RightButton == ButtonState.Pressed &&
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.LeftButton == ButtonState.Pressed; break;
                    case BoutonSouris.DroiteEtMilieu: return
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.RightButton == ButtonState.Pressed &&
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.MiddleButton == ButtonState.Pressed; break;
                    case BoutonSouris.MilieuEtGauche: return
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.LeftButton == ButtonState.Pressed &&
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.MiddleButton == ButtonState.Pressed; break;
                    case BoutonSouris.MilieuHaut: return
                        (GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.ScrollWheelValue -
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisPrecedent.ScrollWheelValue) < 0; break;
                    case BoutonSouris.MilieuBas: return
                        (GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.ScrollWheelValue -
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisPrecedent.ScrollWheelValue) > 0; break;
                }
            }
#endif

            return false;
        }


        //
        // Est-ce que la touche est présentement relâchée
        //

        public bool estRelachee(Z key, PlayerIndex indexJoueur)
        {
            if (Desactive)
                return false;

            if (!Core.Visuel.Facade.sceneEnFocus(NomScene))
                return false;

            if (TouchesTraitees[indexJoueur] != null && !TouchesTraitees[indexJoueur].Contains(key))
                return false;

            if (TouchesIgnorees[indexJoueur] != null && TouchesIgnorees[indexJoueur].Contains(key))
                return false;

            object temp = key;

            if (key is Keys)
            {
                return Keyboard.GetState().IsKeyUp((Keys)temp);
            }

            else if (key is Buttons)
            {
                return GamePad.GetState(indexJoueur).IsButtonUp((Buttons)temp);
            }

#if WINDOWS && !MANETTE_WINDOWS
            else if (key is BoutonSouris)
            {
                BoutonSouris bouton = (BoutonSouris) temp;

                switch (bouton)
                {
                    case BoutonSouris.Droite: return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.RightButton == ButtonState.Released; break;
                    case BoutonSouris.Gauche: return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.LeftButton == ButtonState.Released; break;
                    case BoutonSouris.Milieu: return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.MiddleButton == ButtonState.Released; break;
                    case BoutonSouris.DroiteEtGauche: return
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.RightButton == ButtonState.Released &&
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.LeftButton == ButtonState.Released; break;
                    case BoutonSouris.DroiteEtMilieu: return
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.RightButton == ButtonState.Released &&
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.MiddleButton == ButtonState.Released; break;
                    case BoutonSouris.MilieuEtGauche: return
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.LeftButton == ButtonState.Released &&
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.MiddleButton == ButtonState.Released; break;
                    case BoutonSouris.MilieuHaut: return
                        (GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.ScrollWheelValue -
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisPrecedent.ScrollWheelValue) == 0; break;
                    case BoutonSouris.MilieuBas: return
                        (GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.ScrollWheelValue -
                        GestionnaireTouches<Keys>.InstanceSouris.EtatSourisPrecedent.ScrollWheelValue) == 0; break;
                }
            }
#endif

            return false;
        }


        //
        // Retourne la position du joystick gauche/droit de la manette
        //

        public Vector2 positionThumbstick(PlayerIndex manette, bool gauche, GamePadDeadZone deadZone)
        {
            if (Desactive)
                return Vector2.Zero;

            if (!Core.Visuel.Facade.sceneEnFocus(NomScene))
                return Vector2.Zero;

            if (gauche)
            {
                if (ThumbstickIgnores[manette] != null && ThumbstickIgnores[manette].Contains(Buttons.LeftStick) ||
                    ThumbstickTraites[manette] != null && !ThumbstickTraites[manette].Contains(Buttons.LeftStick))
                    return Vector2.Zero;
            }
            else
            {
                if (ThumbstickIgnores[manette] != null && ThumbstickIgnores[manette].Contains(Buttons.RightStick) ||
                    ThumbstickTraites[manette] != null && !ThumbstickTraites[manette].Contains(Buttons.RightStick))
                    return Vector2.Zero;
            }

            return (gauche ?
                GamePad.GetState(manette, deadZone).ThumbSticks.Left :
                GamePad.GetState(manette, deadZone).ThumbSticks.Right);
        }


#if WINDOWS && !MANETTE_WINDOWS

        public Vector2 positionDeltaSouris(PlayerIndex manette)
        {
            if (Desactive)
                return Vector2.Zero;

            if (!Core.Visuel.Facade.sceneEnFocus(NomScene))
                return Vector2.Zero;

            if (!SourisTraitee[manette])
                return Vector2.Zero;

            return new Vector2(GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.X, GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.Y) - GestionnaireTouches<Keys>.InstanceSouris.PositionBaseSouris;
        }


        public int getDeltaRoueSouris(PlayerIndex manette)
        {
            if (Desactive)
                return 0;

            if (!Core.Visuel.Facade.sceneEnFocus(NomScene))
                return 0;

            return GestionnaireTouches<Keys>.InstanceSouris.EtatSourisMaintenant.ScrollWheelValue - GestionnaireTouches<Keys>.InstanceSouris.EtatSourisPrecedent.ScrollWheelValue;
        }
#endif
    }
}

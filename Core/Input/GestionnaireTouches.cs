//=====================================================================
//
// Gestion des touches (surtout pour les touches qui doivent être
// pesées/relachées)
//
//=====================================================================

namespace Core.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.GamerServices;

    class GestionnaireTouches<T>
    {
        //=====================================================================
        // Attributs
        //=====================================================================

#if WINDOWS && !MANETTE_WINDOWS
        private static GestionnaireTouches<Keys> instance = new GestionnaireTouches<Keys>();
        private static GestionnaireTouches<BoutonSouris> instance2 = new GestionnaireTouches<BoutonSouris>();
#else
        private static GestionnaireTouches<Buttons> instance = new GestionnaireTouches<Buttons>();
#endif

        private Dictionary<String, TouchesScene<T>> scenes = new Dictionary<String, TouchesScene<T>>();

        private Dictionary<PlayerIndex, Dictionary<T, bool>> Touches =
            new Dictionary<PlayerIndex, Dictionary<T, bool>>();             //état des touches (pesées ou non)

        public bool Desactive { get; set; }                                 // toutes les touches sont ignorées
        private bool desactiveBackup;                                       // Utilisé pour la désactivation des touches quand le Guide est ouvert
        private bool backupDone = false;                                    // Idem
        private bool changementDone = false;                                // Idem

        public MouseState EtatSourisPrecedent, EtatSourisMaintenant;
        public Vector2 PositionBaseSouris;


        //=====================================================================
        // Getters / Setters
        //=====================================================================

        public T[] GetTouchesTraitees(PlayerIndex indexJoueur, String nomScene)
        {
            return scenes[nomScene].TouchesTraitees[indexJoueur].ToArray();
        }

        public void SetTouchesTraitees(PlayerIndex indexJoueur, List<T> touchesTraitees, String nomScene)
        {
            scenes[nomScene].TouchesTraitees[indexJoueur] = touchesTraitees;
        }

        public T[] GetTouchesIgnorees(PlayerIndex indexJoueur, String nomScene)
        {
            return scenes[nomScene].TouchesIgnorees[indexJoueur].ToArray();
        }

        public void SetTouchesIgnorees(PlayerIndex indexJoueur, List<T> touchesIgnorees, String nomScene)
        {
            scenes[nomScene].TouchesIgnorees[indexJoueur] = touchesIgnorees;
        }

        public Buttons[] GetThumbsticksIgnores(PlayerIndex indexJoueur, String nomScene)
        {
            return scenes[nomScene].ThumbstickIgnores[indexJoueur].ToArray();
        }

        public void SetThumbsticksIgnores(PlayerIndex indexJoueur, List<Buttons> thumbstickIgnores, String nomScene)
        {
            scenes[nomScene].ThumbstickIgnores[indexJoueur] = thumbstickIgnores;
        }

        public void SetThumbsticksTraites(PlayerIndex indexJoueur, List<Buttons> thumbstickTraites, String nomScene)
        {
            scenes[nomScene].ThumbstickTraites[indexJoueur] = thumbstickTraites;
        }

        public void SetSourisTraitee(PlayerIndex indexJoueur, bool considerer, string nomScene)
        {
            scenes[nomScene].SourisTraitee[indexJoueur] = considerer;
        }


        //=====================================================================
        // Constructeur
        //=====================================================================

        private GestionnaireTouches()
        {
            Desactive = false;

            for (PlayerIndex manette = PlayerIndex.One; manette <= PlayerIndex.Four; manette++)
            {
                Touches.Add(manette, new Dictionary<T, bool>());
            }

            Visuel.Facade.etreNotifierTransition(transitionDemarree, transitionTerminee);
        }


        //=====================================================================
        // Gestion des événements
        //=====================================================================

        private void transitionDemarree(object sender, EventArgs e)
        {
            this.Desactive = true;
        }


        private void transitionTerminee(object sender, EventArgs e)
        {
            this.Desactive = false;
        }


        //=====================================================================
        // Services
        //=====================================================================

        public void ajouterScene(String nomScene)
        {
            scenes.Add(nomScene, new TouchesScene<T>(nomScene));
        }


#if WINDOWS && !MANETTE_WINDOWS
        public static GestionnaireTouches<Keys> Instance
        {
            get { return instance; }
        }

        public static GestionnaireTouches<BoutonSouris> InstanceSouris
        {
            get { return instance2; }
        }
#else
        public static GestionnaireTouches<Buttons> Instance
        {
            get { return instance; }
        }
#endif


        //
        // Activer les touches. null == toutes les scènes sont activées
        //

        public void activer(String nomScene)
        {
            if (nomScene == null)
            {
                foreach (var kvp in scenes)
                    kvp.Value.Desactive = true;

                return;
            }

            scenes[nomScene].Desactive = false;
        }


        //
        // Desactiver les touches. null == toutes les scènes sont désactivées
        //

        public void desactiver(String nomScene)
        {
            if (nomScene == null)
            {
                foreach (var kvp in scenes)
                    kvp.Value.Desactive = false;

                return;
            }

            scenes[nomScene].Desactive = true;
        }


        //
        // Est-ce que la touche a été pesée/relachée ?
        // Prise en compte de la scène dans laquelle s'effectue la requête
        //

        public bool estPeseeUneSeuleFois(T key, PlayerIndex indexJoueur, String nomScene)
        {
            if (Desactive)
                return false;

            if (scenes[nomScene].estPesee(key, indexJoueur))
            {
                if (!Touches[indexJoueur].ContainsKey(key))
                {
                    Touches[indexJoueur].Add(key, true);
                    return true;
                }

                else if (Touches[indexJoueur][key] == true)
                    return true;
            }

            return false;
        }


        //
        // Est-ce que la touches est présentement pesée
        //

        public bool estPesee(T key, PlayerIndex indexJoueur, String nomScene)
        {
            if (Desactive)
                return false;

            if (nomScene == "")
            {
                object temp = key;

                if (key is Keys)
                {
                    return Keyboard.GetState().IsKeyDown((Keys)temp);
                }

                else if (key is Buttons)
                {
                    return GamePad.GetState(indexJoueur).IsButtonDown((Buttons)temp);
                }

                return false;
            }

            return scenes[nomScene].estPesee(key, indexJoueur);
        }


        //
        // A utiliser lorsqu'on veut que la touche ne soit plus traitée durant
        // le tick (par exemple, pour la pause)
        //

        public void keyCantBePressedAgainThisTick(T key, PlayerIndex indexJoueur)
        {
            Touches[indexJoueur][key] = false;
        }


        //
        // Est-ce que la touche est présentement relâchée
        // Prise en compte de la scène dans laquelle s'effectue la requête
        //

        public bool estRelachee(T key, PlayerIndex indexJoueur, String nomScene)
        {
            if (Desactive)
                return false;

            return scenes[nomScene].estRelachee(key, indexJoueur);
        }


        //
        // Retourne la position du joystick gauche/droit de la manette
        // Prise en compte de la scène dans laquelle s'effectue la requête
        //

        public Vector2 positionThumbstick(PlayerIndex manette, bool gauche, GamePadDeadZone deadZone, String nomScene)
        {
            if (Desactive)
                return Vector2.Zero;

            return scenes[nomScene].positionThumbstick(manette, gauche, deadZone);
        }


#if WINDOWS && !MANETTE_WINDOWS

        public Vector2 positionDeltaSouris(PlayerIndex manette, String nomScene)
        {
            if (Desactive)
                return Vector2.Zero;

            return scenes[nomScene].positionDeltaSouris(manette);
        }

        public int getDeltaRoueSouris(PlayerIndex manette, String nomScene)
        {
            if (Desactive)
                return 0;

            return scenes[nomScene].getDeltaRoueSouris(manette);
        }
#endif


        //=====================================================================
        // Update
        //=====================================================================

        public void Update(GameTime gameTime)
        {
#if XBOX
            // Désactive les touches si le Guide est visible, remet leur ancien état ensuite
            if (Guide.IsVisible)
            {
                if (!Desactive) // Survient aussi si le jeu réactive les touches alors que le Guide est ouvert (après l'animation de mort par exemple)
                {
                    desactiveBackup = Desactive;
                    Desactive = true;
                    changementDone = false;
                }
             }
            else if (!changementDone)
            {
                Desactive = desactiveBackup;
                changementDone = true;
            }


            // Désactive les touches si le Guide est visible, les active sinon
            if (Guide.IsVisible)
            {
                if (!backupDone)
                {
                    Desactive = true;
                    backupDone = true;
                    changementDone = false;
                }
                else if (!Desactive) // Survient si le jeu réactive les touches alors que le Guide est ouvert (après l'animation de mort par exemple)
                    Desactive = true;
            }
            else if (!changementDone)
            {
                Desactive = desactiveBackup;
                changementDone = true;
                backupDone = false;
            }
#endif

            List<T> toDelete = new List<T>();

            foreach (KeyValuePair<PlayerIndex, Dictionary<T, bool>> paire in Touches)
            {
                foreach (KeyValuePair<T, bool> kv in paire.Value)
                {
                    if (estRelachee(kv.Key, paire.Key))
                        toDelete.Add(kv.Key);
                }

                for (int i = 0; i < toDelete.Count; i++)
                {
                    Touches[paire.Key].Remove(toDelete[i]);
                }

                T[] wTouches = Touches[paire.Key].Keys.ToArray();

                for (int i = 0; i < wTouches.Length; i++)
                    Touches[paire.Key][wTouches[i]] = false;
            }

            EtatSourisPrecedent = EtatSourisMaintenant;
            EtatSourisMaintenant = Mouse.GetState();
        }

        private bool estRelachee(T key, PlayerIndex indexJoueur)
        {
            if (!Touches[indexJoueur].ContainsKey(key))
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
                BoutonSouris bouton = (BoutonSouris)temp;

                switch (bouton)
                {
                    case BoutonSouris.Droite: return EtatSourisMaintenant.RightButton == ButtonState.Released; break;
                    case BoutonSouris.Gauche: return EtatSourisMaintenant.LeftButton == ButtonState.Released; break;
                    case BoutonSouris.Milieu: return EtatSourisMaintenant.MiddleButton == ButtonState.Released; break;
                    case BoutonSouris.DroiteEtGauche: return
                        EtatSourisMaintenant.RightButton == ButtonState.Released &&
                        EtatSourisMaintenant.LeftButton == ButtonState.Released; break;
                    case BoutonSouris.DroiteEtMilieu: return
                        EtatSourisMaintenant.RightButton == ButtonState.Released &&
                        EtatSourisMaintenant.MiddleButton == ButtonState.Released; break;
                    case BoutonSouris.MilieuEtGauche: return
                        EtatSourisMaintenant.LeftButton == ButtonState.Released &&
                        EtatSourisMaintenant.MiddleButton == ButtonState.Released; break;
                    case BoutonSouris.MilieuHaut: return (EtatSourisMaintenant.ScrollWheelValue - EtatSourisPrecedent.ScrollWheelValue) == 0; break;
                    case BoutonSouris.MilieuBas: return (EtatSourisMaintenant.ScrollWheelValue - EtatSourisPrecedent.ScrollWheelValue) == 0; break;
                }
            }
#endif

            return false;
        }
    }
}

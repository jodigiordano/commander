//=====================================================================
//
// Scene
// C'est un IVisible qui a son propre carré de sable (buffer) pour
// dessiner d'autres IVisible. Ainsi, de l'extérieur, on utilise
// et on fait subir des transformations à la Scene comme on le fait
// pour un IVisible.
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;
    using Wintellect.PowerCollections;
    using Core.Input;
    using Microsoft.Xna.Framework.Input;

    public abstract class Scene : IVisible, InputListener
    {
        //=====================================================================
        // Getters / Setters statiques
        //=====================================================================

        internal static ContentManager Contenu;


        //=====================================================================
        // Attributs internes
        //=====================================================================

        private enum Type { Feuille, Noyau, Branche }

        private bool DernierElementEnFocus;             // Est-ce que le dernier élément affiché est dans le focus
        private SpriteBatch Batch;                      // Gestion des sprites avant l'écriture dans le buffer
        private TypeMelange DernierMelange;
        internal Scene parent;
        internal bool EstUpdateCeTick;
        private Type type;
        private GestionnaireAnimations animations;
        private GestionnaireEffets effets;
        private GestionnaireParticules particules;


        //=====================================================================
        // Attributs services
        //=====================================================================

        //public List<IScenable> ElementsAffiches;                    // Éléments à afficher
        private OrderedBag<IScenable> ElementsAffiches;
        
        public bool EstVisible;                                     // Doit-on afficher le contenu (utile pour les Gestionnaires)
        public bool EnPause;                                        // Doit-on mettre à jour la logique (utile pour les Gestionnaires)
        public bool EnFocus;                                        // La scène répond-t-elle aux touches, est-ce sa musique qui joue (utile pour les Gestionnaires)
        public virtual bool EstTerminee { get; protected set; }

        // Peut être redéfini par des scènes souhaitant mettre leurs animations/particules/effets ailleurs
        public virtual GestionnaireAnimations Animations { get { return animations; } }
        public virtual GestionnaireParticules Particules { get { return particules; } }
        public virtual GestionnaireEffets Effets { get { return effets; } }


        //=====================================================================
        // Constructeur
        //=====================================================================

        private Scene() { }


        /// <summary>
        /// Créer une sous-scène (sans buffer ni caméra)
        /// </summary>
        public Scene(Vector2 position, string nomSceneParent)
            : base(null, new Vector3(position.X, position.Y, 0))
        {
            this.parent = GestionnaireScenes.Instance.recuperer(nomSceneParent);

            // Si un parent est spécifié, les éléments de la scène s'affichent dans celle-ci
            tampon = parent.Tampon;

            type = Type.Feuille;

            Initialize(position);
        }


        /// <summary>
        /// Créer une scène parent (avec buffer et caméra)
        /// </summary>
        public Scene(Vector2 position, int hauteur, int largeur)
            : base(null, new Vector3(position.X, position.Y, 0))
        {
            //tampon = new Tampon(hauteur, largeur);
            //tampon = GestionnaireTampons.Instance.recuperer(largeur, hauteur);
            tampon = GestionnaireScenes.Instance.Tampon;

            type = Type.Noyau;

            Initialize(position);
        }


        /// <summary>
        /// Créer une scène noeud (avec buffer et caméra) mais appartenant tout de même à une scène parent
        /// </summary>
        public Scene(Vector2 position, int hauteur, int largeur, string nomSceneParent)
            : base(null, new Vector3(position.X, position.Y, 0))
        {
            //tampon = new Tampon(hauteur, largeur);
            //tampon = GestionnaireTampons.Instance.recuperer(largeur, hauteur);
            tampon = GestionnaireScenes.Instance.Tampon;

            this.parent = GestionnaireScenes.Instance.recuperer(nomSceneParent);

            type = Type.Branche;

            Initialize(position);
        }


        private void Initialize(Vector2 position)
        {
            //initialise la liste des éléments à afficher
            //note: capacité élevée pour éviter de grandir la liste inutilement
            //ElementsAffiches = new List<IScenable>(1000);
            ElementsAffiches = new OrderedBag<IScenable>(ComparerIScenables);

            Batch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
            DernierMelange = TypeMelange.Alpha;
            Origine = Centre;
            EstUpdateCeTick = false;

            camera = new Camera();
            camera.Origine = new Vector2(this.Tampon.Largeur / 2.0f, this.Tampon.Hauteur / 2.0f);
            //camera.Dimension = new Vector2(this.Tampon.Largeur, this.Tampon.Hauteur);

            Nom = nom;

            DernierElementEnFocus = false;

            animations = new GestionnaireAnimations();
            effets = new GestionnaireEffets();
            particules = new GestionnaireParticules(this);

            Core.Input.Facade.AddListener(this);
        }


        //=====================================================================
        // Services
        //=====================================================================

        private String nom;
        public String Nom
        {
            get { return nom; }
            set
            {
                nom = value;
                Camera.Nom = Nom + ".Camera";
            }
        }


        public override Texture2D Texture
        {
            get { return Tampon.Texture; }
        }


        public int Largeur
        {
            get { return Tampon.Largeur; }
        }


        public int Hauteur
        {
            get { return Tampon.Hauteur; }
        }


        public override Vector2 Centre
        {
            get { return Tampon.Centre; }
        }


        public bool GarderContenu
        {
            set
            {
                if (type == Type.Feuille)
                    throw new Exception("Pas a toi a decider !");

                Tampon.GarderContenu = value;
            }
        }


        public Color CouleurDeFond
        {
            set 
            {
                if (type == Type.Feuille)
                    throw new Exception("Pas a toi a decider !");

                Tampon.CouleurDeFond = value;
            }
        }

        private Tampon tampon;
        internal Tampon Tampon
        {
            get { return (type == Type.Feuille) ? parent.Tampon : this.tampon; }
        }

        internal bool EstProprietaireDeSonTampon
        {
            get { return (type == Type.Noyau) && tampon != null; }
        }

        private Camera camera;
        public Camera Camera
        {
            get
            {
                return (type == Type.Feuille) ? parent.Camera : this.camera;
            }

            set
            {
                if (type == Type.Feuille)
                    throw new Exception("Pas a toi a decider !");

                this.camera = value;
            }
        }

        internal int Niveau
        {
            get
            {
                int niveau = 0;
                Scene parent = this.parent;

                while (parent != null)
                {
                    niveau++;
                    parent = parent.parent;
                }

                return niveau;
            }
        }


        /// <summary>
        /// Dessiner les éléments dans le buffer
        /// </summary>
        public void Draw()
        {
            UpdateVisuel();

            ajouterScenable(animations.Composants);
            ajouterScenable(particules.Composants);

            EstUpdateCeTick = true;

            if (type == Type.Feuille)
                return;

            DernierElementEnFocus = true;

            Tampon.EcrireDebut();

            Batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Camera.Transformee);
            changerBlendMode(TypeMelange.Alpha); // effectuer une fois avec le blend le plus commun pour s'assurer d'avoir un state cohérent

            for (int i = 0; i < ElementsAffiches.Count; i++)
            {
                if (ElementsAffiches[i].Melange != DernierMelange)
                {
                    this.Batch.End();
                    this.Batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Camera.Transformee);
                    changerBlendMode(ElementsAffiches[i].Melange);
                }

                ElementsAffiches[i].Draw(this.Batch);
            }

            Batch.End();

            Batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Transformee);
            changerBlendMode(TypeMelange.Default);
            Batch.End();

            // effacer la liste des éléments à afficher
            this.ElementsAffiches.Clear();

            //if (type == Type.Branche)
            //{
            //    this.Scene = parent;
            //    parent.ajouterScenable(this);
            //}
        }


        /// <summary>
        /// Ajoute un élément dans la scène
        /// </summary>
        /// <param name="element">Élément ajouté</param>
        public void ajouterScenable(IScenable element)
        {
            if (type == Type.Feuille)
            {
                parent.ajouterScenable(element);
            }

            else
            {
                ElementsAffiches.Add(element);

                if (element.Composants != null)
                    ajouterScenable(element.Composants);
            }
        }


        /// <summary>
        /// Ajoute des éléments dans la scène
        /// </summary>
        /// <param name="elements">Éléments ajoutés</param>
        public void ajouterScenable(List<IScenable> elements)
        {
            if (type == Type.Feuille)
            {
                parent.ajouterScenable(elements);
            }

            else
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i] == null) //ARK ARK ARK
                        continue;

                    ElementsAffiches.Add(elements[i]);

                    if (elements[i].Composants != null)
                        ajouterScenable(elements[i].Composants);
                }
            }
        }


        /// <summary>
        /// Ajoute des éléments dans la scène
        /// </summary>
        /// <param name="elements">Éléments ajoutés</param>
        public void ajouterScenable(List<IVisible> elements)
        {
            if (type == Type.Feuille)
            {
                parent.ajouterScenable(elements);
            }

            else
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    ElementsAffiches.Add(elements[i]);

                    if (elements[i].Composants != null)
                        ajouterScenable(elements[i].Composants);
                }
            }
        }


        /// <summary>
        /// Ajouter des éléments dans la scène
        /// </summary>
        /// <param name="elements">Éléments ajoutés</param>
        public void ajouterScenable(List<Animation> elements)
        {
            if (type == Type.Feuille)
            {
                parent.ajouterScenable(elements);
            }

            else
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    ElementsAffiches.Add(elements[i]);

                    if (elements[i].Composants != null)
                        ajouterScenable(elements[i].Composants);
                }
            }
        }


        /// <summary>
        /// Mettre à jour la logique associée à la scène
        /// </summary>
        public void Update(GameTime gameTime)
        {
            this.Camera.Update(gameTime);

            _gameTime = gameTime;

#if XBOX
            Preferences.ThreadParticules.AddTask(new ThreadTask(doUpdateAsynchrone));
#else
            doUpdateAsynchrone();
#endif
            UpdateLogique(gameTime);
            effets.Update(gameTime);
#if XBOX
            //while (Preferences.ThreadLogique.Travaille()) { }
            while (Preferences.ThreadParticules.Travaille()) { }
#endif
        }

        private GameTime _gameTime;
        private void doUpdateAsynchrone()
        {
            particules.Update(_gameTime);
            animations.Update(_gameTime);
        }


        public void Dispose()
        {
            Core.Input.Facade.RemoveListener(this);
        }


        protected abstract void UpdateLogique(GameTime gameTime);


        protected abstract void UpdateVisuel();


        //=====================================================================
        // Événements
        //=====================================================================

        public virtual void onTransitionTowardFocus() { }


        public virtual void onFocus() { }


        public virtual void onFocusLost() { }


        //=====================================================================
        // Helpers
        //=====================================================================

        /// <summary>
        /// Tri les éléments de la scène
        /// </summary>
        /// 
        private int ComparerIScenables(IScenable e1, IScenable e2)
        {
            // tri sur la priorité d'affichage pour un même Z
            if (e1.PrioriteAffichage < e2.PrioriteAffichage)
                return 1;

            if (e1.PrioriteAffichage > e2.PrioriteAffichage)
                return -1;

            // tri final sur le hash code
            if (e1.GetHashCode() > e2.GetHashCode())
                return 1;

            if (e1.GetHashCode() < e2.GetHashCode())
                return -1;

            // impossible de se rendre ici :)
            // mais pas d'exceptions levés parce que sa pourrait être deux fois le même objet
            return 0;
        }


        /// <summary>
        /// Bascule d'un blend mode à un autre lors de l'écriture dans le buffer
        /// </summary>
        /// <param name="typeMelange">Blend mode à utiliser</param>
        private void changerBlendMode(TypeMelange typeMelange)
        {
            DernierMelange = typeMelange;

            switch (typeMelange)
            {
                case TypeMelange.Additif:
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
                    break;

                case TypeMelange.Multiplicatif:
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Max;
                    break;

                default:
                case TypeMelange.Default:
                case TypeMelange.Alpha:
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
                    break;

                case TypeMelange.Soustraire:
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                    Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.BlendFunction = BlendFunction.ReverseSubtract;
                    break;

                case TypeMelange.Aucun:
                    break;
            }

            Batch.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
            Batch.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
            Batch.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
        }


        public bool Active
        {
            get { return EstVisible && !EnPause && EnFocus; }
        }


        public virtual void doKeyPressedOnce(PlayerIndex inputIndex, Keys key) {}
        public virtual void doKeyReleased(PlayerIndex inputIndex, Keys key) {}
        public virtual void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button) { }
        public virtual void doMouseButtonReleased(PlayerIndex inputIndex, MouseButton button) { }
        public virtual void doMouseScrolled(PlayerIndex inputIndex, int delta) { }
        public virtual void doMouseMoved(PlayerIndex inputIndex, Vector3 delta) { }
        public virtual void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button) { }
        public virtual void doGamePadButtonReleased(PlayerIndex inputIndex, Buttons button) { }
        public virtual void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta) { }
    }
}

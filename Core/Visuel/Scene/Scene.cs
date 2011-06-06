namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Wintellect.PowerCollections;


    public abstract class Scene : InputListener
    {
        public bool Active { get; set; }

        internal VisualBuffer Tampon { get; private set; }
        public Camera Camera { get; private set; }

        public Texture2D Texture    { get { return Tampon.Texture; } }
        public int Largeur          { get { return Tampon.Largeur; } }
        public int Hauteur          { get { return Tampon.Hauteur; } }
        public Vector2 Centre       { get { return Tampon.Centre; } }

        public virtual bool EstTerminee { get; protected set; }
        public virtual AnimationsController Animations { get { return animations; } }
        public virtual ParticlesController Particules { get { return particules; } }
        public virtual EffectsController Effets { get { return effets; } }

        private SpriteBatch Batch;
        private TypeBlend DernierMelange;
        internal bool EstUpdateCeTick;
        private AnimationsController animations;
        private EffectsController effets;
        private ParticlesController particules;
        private OrderedBag<IScenable> ElementsAffiches;
        private String nom;
        private TransitionType transition;


        public Scene(Vector2 position, int hauteur, int largeur)
        {
            Tampon = Facade.ScenesController.Buffer;
            Active = false;
            ElementsAffiches = new OrderedBag<IScenable>(ComparerIScenables);
            Batch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
            DernierMelange = TypeBlend.Alpha;
            EstUpdateCeTick = false;
            Camera = new Camera();
            Camera.Origin = new Vector2(this.Tampon.Largeur / 2.0f, this.Tampon.Hauteur / 2.0f);
            Nom = nom;
            animations = new AnimationsController(this);
            effets = new EffectsController();
            particules = new ParticlesController(this);
            EphemereGames.Core.Input.Facade.AddListener(this);
            Transition = TransitionType.None;
        }


        public String Nom
        {
            get { return nom; }
            set
            {
                nom = value;
                Camera.Name = Nom + ".Camera";
            }
        }


        public TransitionType Transition
        {
            get { return transition; }
            set
            {
                transition = value;

                if (value != TransitionType.None)
                    InitializeTransition(value);
            }
        }


        public bool GarderContenu
        {
            set
            {
                Tampon.GarderContenu = value;
            }
        }


        public Color CouleurDeFond
        {
            set 
            {
                Tampon.CouleurDeFond = value;
            }
        }


        public void Draw()
        {
            UpdateVisuel();

            ajouterScenable(animations.Components);
            ajouterScenable(particules.Particles);

            EstUpdateCeTick = true;

            Tampon.EcrireDebut();

            BlendState b = changerBlendMode(TypeBlend.Alpha);

            Batch.Begin(SpriteSortMode.Deferred, b, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.Transform); //todo

            for (int i = 0; i < ElementsAffiches.Count; i++)
            {
                if (ElementsAffiches[i].Blend != DernierMelange)
                {
                    this.Batch.End();
                    Batch.Begin(SpriteSortMode.Deferred, changerBlendMode(ElementsAffiches[i].Blend), SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.Transform); //todo
                }

                ElementsAffiches[i].Draw(this.Batch);
            }

            Batch.End();


            this.ElementsAffiches.Clear();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                new Vector2(Camera.Position.X, Camera.Position.Y),
                null,
                Color.White,
                Camera.Rotation,
                Camera.Origin,
                1,
                SpriteEffects.None,
                0);
        }


        public void ajouterScenable(IScenable element)
        {
            ElementsAffiches.Add(element);

            if (element.Components != null)
                ajouterScenable(element.Components);
        }


        public void ajouterScenable(List<IScenable> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == null) //ARK ARK ARK
                    continue;

                ElementsAffiches.Add(elements[i]);

                if (elements[i].Components != null)
                    ajouterScenable(elements[i].Components);
            }
        }


        public void ajouterScenable(List<IVisible> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ElementsAffiches.Add(elements[i]);

                if (elements[i].Components != null)
                    ajouterScenable(elements[i].Components);
            }
        }


        public void ajouterScenable(List<Animation> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ElementsAffiches.Add(elements[i]);

                if (elements[i].Components != null)
                    ajouterScenable(elements[i].Components);
            }
        }


        public void ajouterScenable(List<Particle> elements)
        {
            for (int i = 0; i < elements.Count; i++)
                ElementsAffiches.Add(elements[i]);
        }


        public void Update(GameTime gameTime)
        {
            this.Camera.Update(gameTime);

            _gameTime = gameTime;

#if XBOX
            Preferences.ThreadParticules.AddTask(new ThreadTask(doUpdateAsynchrone));
#else
            doUpdateAsynchrone();
#endif
            if (Transition != TransitionType.None)
                UpdateTransition(gameTime);

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
            EphemereGames.Core.Input.Facade.RemoveListener(this);
        }


        private int ComparerIScenables(IScenable e1, IScenable e2)
        {
            if (e1.VisualPriority < e2.VisualPriority)
                return 1;

            if (e1.VisualPriority > e2.VisualPriority)
                return -1;

            if (e1.GetHashCode() > e2.GetHashCode())
                return 1;

            if (e1.GetHashCode() < e2.GetHashCode())
                return -1;

            return 0;
        }


        private BlendState changerBlendMode(TypeBlend typeMelange)
        {
            DernierMelange = typeMelange;

            switch (typeMelange)
            {
                case TypeBlend.Add:
                    return BlendState.Additive;
                    break;

                case TypeBlend.Multiply:
                    return CustomBlends.Multiply;
                    break;

                default:
                case TypeBlend.Default:
                case TypeBlend.Alpha:
                    return CustomBlends.Alpha;
                    break;

                case TypeBlend.Substract:
                    return CustomBlends.Substract;
                    break;

                case TypeBlend.None:
                    break;
            }

            return CustomBlends.Alpha;
        }


        protected abstract void UpdateLogique(GameTime gameTime);
        protected abstract void UpdateVisuel();
        protected abstract void InitializeTransition(TransitionType type);
        protected abstract void UpdateTransition(GameTime gameTime);
        public virtual void onTransitionTowardFocus() { }
        public virtual void onFocus() { }
        public virtual void onFocusLost() { }
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

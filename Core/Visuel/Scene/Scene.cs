﻿namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;


    public abstract class Scene : InputListener
    {
        public bool Active              { get; set; }

        internal VisualBuffer Buffer    { get; private set; }
        public Camera Camera            { get; private set; }

        public Texture2D Texture        { get { return Buffer.Texture; } }
        public int Width                { get { return Buffer.Largeur; } }
        public int Height               { get { return Buffer.Hauteur; } }
        public Vector2 Center           { get { return Buffer.Centre; } }

        public virtual bool IsFinished                      { get; protected set; }
        public virtual AnimationsController Animations      { get; protected set; }
        public virtual ParticlesController Particles        { get; protected set; }
        public virtual EffectsController Effects            { get; protected set; }

        private SpriteBatch Batch;
        private TypeBlend LastBlend;
        internal bool UpdatedThisTick;
        private OrderedSet<IScenable> ToDraw;
        private String name;
        private TransitionType transition;


        public Scene(Vector2 position, int hauteur, int largeur)
        {
            Buffer = Facade.ScenesController.Buffer;
            Active = false;
            ToDraw = new OrderedSet<IScenable>(new IScenableComparer());
            Batch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
            LastBlend = TypeBlend.Alpha;
            UpdatedThisTick = false;
            Camera = new Camera();
            Camera.Origin = new Vector2(this.Buffer.Largeur / 2.0f, this.Buffer.Hauteur / 2.0f);
            Nom = name;
            Animations = new AnimationsController(this);
            Effects = new EffectsController();
            Particles = new ParticlesController(this);
            EphemereGames.Core.Input.Facade.AddListener(this);
            Transition = TransitionType.None;
        }


        public String Nom
        {
            get { return name; }
            set
            {
                name = value;
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


        public void Draw()
        {
            UpdateVisual();

            ajouterScenable(Animations.Components);
            ajouterScenable(Particles.Particles);

            UpdatedThisTick = true;

            Buffer.EcrireDebut();

            BlendState b = SwitchBlendMode(TypeBlend.Alpha);

            Batch.Begin(SpriteSortMode.Deferred, b, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.Transform); //todo

            foreach (var scenable in ToDraw)
            {
                if (scenable.Blend != LastBlend)
                {
                    this.Batch.End();
                    Batch.Begin(SpriteSortMode.Deferred, SwitchBlendMode(scenable.Blend), SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.Transform); //todo
                }

                scenable.Draw(Batch);
            }

            Batch.End();


            this.ToDraw.Clear();
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
            ToDraw.Add(element);

            if (element.Components != null)
                ajouterScenable(element.Components);
        }


        public void Add(IScenable element)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == null) //ARK ARK ARK
                    continue;

                ToDraw.Add(elements[i]);

                if (elements[i].Components != null)
                    ajouterScenable(elements[i].Components);
            }
        }


        public void Add(List<IScenable> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ToDraw.Add(elements[i]);

                if (elements[i].Components != null)
                    ajouterScenable(elements[i].Components);
            }
        }


        public void Add(List<IVisible> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ToDraw.Add(elements[i]);

                if (elements[i].Components != null)
                    ajouterScenable(elements[i].Components);
            }
        }


        public void Remove(IScenable element)
        {
            for (int i = 0; i < elements.Count; i++)
                ToDraw.Add(elements[i]);
        }


        public void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);

            Particles.Update(gameTime);
            Animations.Update(gameTime);

            if (Transition != TransitionType.None)
                UpdateTransition(gameTime);

            UpdateLogic(gameTime);
            Effects.Update(gameTime);
        }


        public void Dispose()
        {
            EphemereGames.Core.Input.Facade.RemoveListener(this);
        }


        private BlendState SwitchBlendMode(TypeBlend blend)
        {
            LastBlend = blend;

            BlendState newBlend = BlendState.AlphaBlend;

            switch (blend)
            {
                case TypeBlend.Add:
                    newBlend = BlendState.Additive;
                    break;

                case TypeBlend.Multiply:
                    newBlend = CustomBlends.Multiply;
                    break;

                default:
                case TypeBlend.Default:
                case TypeBlend.Alpha:
                    newBlend = CustomBlends.Alpha;
                    break;

                case TypeBlend.Substract:
                    newBlend = CustomBlends.Substract;
                    break;
            }

            return newBlend;
        }


        protected abstract void UpdateLogic(GameTime gameTime);
        protected abstract void UpdateVisual();
        protected abstract void InitializeTransition(TransitionType type);
        protected abstract void UpdateTransition(GameTime gameTime);

        public virtual void OnTransitionTowardFocus() { }
        public virtual void OnFocus() { }
        public virtual void OnFocusLost() { }

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

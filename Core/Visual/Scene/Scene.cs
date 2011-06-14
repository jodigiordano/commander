namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physics;
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

        public virtual bool IsFinished                                      { get; protected set; }
        public virtual AnimationsController Animations                      { get; protected set; }
        public virtual ParticlesController Particles                        { get; protected set; }
        public virtual EffectsController<IPhysicalObject> PhysicalEffects   { get; protected set; }
        public virtual EffectsController<IVisual> VisualEffects             { get; protected set; }

        private SpriteBatch Batch;
        private TypeBlend LastBlend;
        internal bool UpdatedThisTick;
        private List<IScenable> ToDraw;
        private string name;
        private TransitionType transition;


        public Scene(Vector2 position, int hauteur, int largeur)
        {
            Buffer = Visuals.ScenesController.Buffer;
            Active = false;
            ToDraw = new List<IScenable>();
            Batch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
            LastBlend = TypeBlend.Alpha;
            UpdatedThisTick = false;
            Camera = new Camera();
            Camera.Origin = new Vector2(this.Buffer.Largeur / 2.0f, this.Buffer.Hauteur / 2.0f);
            Nom = name;
            Animations = new AnimationsController(this);
            PhysicalEffects = new EffectsController<IPhysicalObject>();
            VisualEffects = new EffectsController<IVisual>();
            Particles = new ParticlesController(this);
            EphemereGames.Core.Input.Input.AddListener(this);
            Transition = TransitionType.None;
        }


        public string Nom
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

            Animations.Draw();
            Add(Particles.Particles);

            UpdatedThisTick = true;

            ToDraw.Sort(IScenableComparer.Default);

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

            ToDraw.Clear();
        }


        //public virtual void Show() { }
        //public virtual void Hide() { }


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


        //public void Clear()
        //{
        //    ToDraw.Clear();
        //}


        public void Add(IScenable element)
        {
            ToDraw.Add(element);
        }


        public void Add(List<IScenable> elements)
        {
            foreach (var e in elements)
                Add(e);
        }


        public void Add(List<IVisible> elements)
        {
            foreach (var e in elements)
                Add(e);
        }


        public void Add(List<Particle> elements)
        {
            foreach (var e in elements)
                Add(e);
        }


        //public void Remove(IScenable element)
        //{
        //    ToDraw.Remove(element);
        //}


        public void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);

            Particles.Update(gameTime);
            Animations.Update(gameTime);

            if (Transition != TransitionType.None)
                UpdateTransition(gameTime);

            UpdateLogic(gameTime);
            PhysicalEffects.Update(gameTime);
            VisualEffects.Update(gameTime);
        }


        public void Dispose()
        {
            EphemereGames.Core.Input.Input.RemoveListener(this);
        }



        public void Clamp(ref Vector3 v)
        {
            v.X = MathHelper.Clamp(v.X, -Width / 2, Width / 2);
            v.Y = MathHelper.Clamp(v.Y, -Height / 2, Height / 2);
        }


        public Vector3 Clamp(Vector3 v)
        {
            v.X = MathHelper.Clamp(v.X, -Width / 2, Width / 2);
            v.Y = MathHelper.Clamp(v.Y, -Height / 2, Height / 2);

            return v;
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

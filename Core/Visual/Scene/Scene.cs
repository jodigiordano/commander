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

    enum DrawMode
    {
        Foreground,
        Background,
        Default
    }


    public abstract class Scene : InputListener
    {
        public bool EnableVisuals { get; set; }
        public bool EnableInputs { get; set; }
        public bool EnableUpdate { get; set; }

        internal VisualBuffer Buffer { get; private set; }
        public Camera Camera { get; private set; }

        internal Texture2D Texture { get { return Buffer.Texture; } }
        public int Width { get { return Buffer.Width; } }
        public int Height { get { return Buffer.Height; } }
        public Vector2 Center { get { return Buffer.Center; } }

        public virtual bool IsFinished { get; protected set; }
        public virtual AnimationsController Animations { get; protected set; }
        public virtual ParticlesController Particles { get; protected set; }
        public virtual EffectsController<IPhysicalObject> PhysicalEffects { get; protected set; }
        public virtual EffectsController<IVisual> VisualEffects { get; protected set; }

        private SpriteBatch Batch;
        private BlendType LastBlend;
        internal bool UpdatedThisTick;
        private List<IScenable> ToDraw;
        private List<IScenable> ToDrawWithoutCameraForeground;
        private List<IScenable> ToDrawWithoutCameraBackground;
        private string name;
        private Matrix IdentityMatrix;
        private DrawMode DrawMode;


        public Scene(int width, int height)
        {
            Buffer = Visuals.ScenesController.Buffer;
            EnableVisuals = false;
            EnableInputs = false;
            EnableUpdate = false;
            ToDraw = new List<IScenable>();
            ToDrawWithoutCameraForeground = new List<IScenable>();
            ToDrawWithoutCameraBackground = new List<IScenable>();
            Batch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
            LastBlend = BlendType.Alpha;
            UpdatedThisTick = false;
            Camera = new Camera(new Vector2(width, height));
            Camera.Origin = new Vector2(width / 2.0f, height / 2.0f);
            Name = name;
            Animations = new AnimationsController(this);
            PhysicalEffects = new EffectsController<IPhysicalObject>();
            VisualEffects = new EffectsController<IVisual>();
            Particles = new ParticlesController(this);
            EphemereGames.Core.Input.Inputs.AddListener(this);
            IdentityMatrix = Matrix.CreateTranslation(width / 2, height / 2, 0);
            DrawMode = DrawMode.Default;
        }


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Camera.Name = Name + ".Camera";
            }
        }


        public Color ClearColor
        {
            set { Buffer.ClearColor = value; }
        }


        internal void Draw()
        {
            UpdateVisual();

            Animations.Draw();
            Add(Particles.Particles);

            UpdatedThisTick = true;

            Buffer.BeginWriting();

            Draw(ToDrawWithoutCameraBackground, ref IdentityMatrix);
            Draw(ToDraw, ref Camera.Transform);
            Draw(ToDrawWithoutCameraForeground, ref IdentityMatrix);
        }


        private void Draw(List<IScenable> toDraw, ref Matrix camera)
        {
            toDraw.Sort(IScenableComparer.Default);

            BlendState b = SwitchBlendMode(BlendType.Alpha);

            Batch.Begin(SpriteSortMode.Deferred, b, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera);

            foreach (var scenable in toDraw)
            {
                if (scenable.Blend != LastBlend)
                {
                    this.Batch.End();
                    Batch.Begin(SpriteSortMode.Deferred, SwitchBlendMode(scenable.Blend), SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera);
                }

                scenable.Draw(Batch);
            }

            Batch.End();

            toDraw.Clear();
        }


        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                Vector2.Zero,
                null,
                Color.White,
                0,
                Camera.Origin,
                1,
                SpriteEffects.None,
                0);
        }


        public void TransiteTo(string to)
        {
            Visuals.Transite(Name, to);
        }


        public void BeginForeground()
        {
            DrawMode = DrawMode.Foreground;
        }


        public void EndForeground()
        {
            DrawMode = DrawMode.Default;

        }


        public void BeginBackground()
        {
            DrawMode = DrawMode.Background;

        }


        public void EndBackground()
        {
            DrawMode = DrawMode.Default;

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


        public void Add(IScenable element)
        {
            switch (DrawMode)
            {
                case DrawMode.Default: ToDraw.Add(element); break;
                case DrawMode.Background: ToDrawWithoutCameraBackground.Add(element); break;
                case DrawMode.Foreground: ToDrawWithoutCameraForeground.Add(element); break;
            }
        }


        public void Update(GameTime gameTime)
        {
            Particles.Update(gameTime);
            Animations.Update(gameTime);

            UpdateLogic(gameTime);
            PhysicalEffects.Update((float) gameTime.ElapsedGameTime.TotalMilliseconds);
            VisualEffects.Update((float) gameTime.ElapsedGameTime.TotalMilliseconds);
        }


        internal void Dispose()
        {
            EphemereGames.Core.Input.Inputs.RemoveListener(this);
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


        private BlendState SwitchBlendMode(BlendType blend)
        {
            LastBlend = blend;

            BlendState newBlend = BlendState.AlphaBlend;

            switch (blend)
            {
                case BlendType.Add:
                    newBlend = BlendState.Additive;
                    break;

                case BlendType.Multiply:
                    newBlend = CustomBlends.Multiply;
                    break;

                default:
                case BlendType.Default:
                case BlendType.Alpha:
                    newBlend = CustomBlends.Alpha;
                    break;

                case BlendType.Substract:
                    newBlend = CustomBlends.Substract;
                    break;
            }

            return newBlend;
        }


        protected abstract void UpdateLogic(GameTime gameTime);
        protected abstract void UpdateVisual();

        public virtual void OnFocus() { }
        public virtual void OnFocusLost() { }

        public virtual void DoKeyPressed(Player player, Keys key) { }
        public virtual void DoKeyPressedOnce(Player player, Keys key) { }
        public virtual void DoKeyReleased(Player player, Keys key) { }
        public virtual void DoMouseButtonPressed(Player player, MouseButton button) { }
        public virtual void DoMouseButtonPressedOnce(Player player, MouseButton button) { }
        public virtual void DoMouseButtonReleased(Player player, MouseButton button) { }
        public virtual void DoMouseScrolled(Player player, int delta) { }
        public virtual void DoMouseMoved(Player player, Vector3 delta) { }
        public virtual void DoGamePadButtonPressedOnce(Player player, Buttons button) { }
        public virtual void DoGamePadButtonReleased(Player player, Buttons button) { }
        public virtual void DoGamePadJoystickMoved(Player player, Buttons button, Vector3 delta) { }
        public virtual void DoPlayerConnected(Player player) { }
        public virtual void DoPlayerDisconnected(Player player) { }
        public virtual void PlayerKeyboardConnectionRequested(Player Player, Keys key) { }
        public virtual void PlayerMouseConnectionRequested(Player Player, MouseButton button) { }
        public virtual void PlayerGamePadConnectionRequested(Player Player, Buttons button) { }
    }
}

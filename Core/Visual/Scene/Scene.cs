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
        public bool EnableVisuals              { get; set; }
        public bool EnableInputs               { get; set; }
        public bool EnableUpdate               { get; set; }

        internal VisualBuffer Buffer    { get; private set; }
        public Camera Camera            { get; private set; }

        internal Texture2D Texture      { get { return Buffer.Texture; } }
        public int Width                { get { return Buffer.Width; } }
        public int Height               { get { return Buffer.Height; } }
        public Vector2 Center           { get { return Buffer.Center; } }

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


        public Scene(Vector2 position, int width, int height)
        {
            Buffer = Visuals.ScenesController.Buffer;
            EnableVisuals = false;
            EnableInputs = false;
            EnableUpdate = false;
            ToDraw = new List<IScenable>();
            Batch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
            LastBlend = TypeBlend.Alpha;
            UpdatedThisTick = false;
            Camera = new Camera();
            Camera.Origin = new Vector2(this.Buffer.Width / 2.0f, this.Buffer.Height / 2.0f);
            Name = name;
            Animations = new AnimationsController(this);
            PhysicalEffects = new EffectsController<IPhysicalObject>();
            VisualEffects = new EffectsController<IVisual>();
            Particles = new ParticlesController(this);
            EphemereGames.Core.Input.Inputs.AddListener(this);
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

            ToDraw.Sort(IScenableComparer.Default);

            Buffer.BeginWriting();

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


        internal void Draw(SpriteBatch spriteBatch)
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


        public void TransiteTo(string to)
        {
            Visuals.Transite(Name, to);
        }


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


        public void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);

            Particles.Update(gameTime);
            Animations.Update(gameTime);

            UpdateLogic(gameTime);
            PhysicalEffects.Update(gameTime);
            VisualEffects.Update(gameTime);
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

        public virtual void OnFocus() { }
        public virtual void OnFocusLost() { }

        public virtual void DoKeyPressedOnce(Player player, Keys key) {}
        public virtual void DoKeyReleased(Player player, Keys key) {}
        public virtual void DoMouseButtonPressedOnce(Player player, MouseButton button) { }
        public virtual void DoMouseButtonReleased(Player player, MouseButton button) { }
        public virtual void DoMouseScrolled(Player player, int delta) { }
        public virtual void DoMouseMoved(Player player, Vector3 delta) { }
        public virtual void DoGamePadButtonPressedOnce(Player player, Buttons button) { }
        public virtual void DoGamePadButtonReleased(Player player, Buttons button) { }
        public virtual void DoGamePadJoystickMoved(Player player, Buttons button, Vector3 delta) { }
        public virtual void DoPlayerConnected(Player player) { }
        public virtual void DoPlayerDisconnected(Player player) { }
        public virtual void PlayerConnectionRequested(Player Player) { }
    }
}

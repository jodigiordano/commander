namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;


    public abstract class Animation : IScenable
    {
        public bool Paused                  { get; set; }
        public float VisualPriority         { get; set; }
        public Scene Scene                  { protected get; set; }
        public Vector3 Position             { get; set; }
        public TypeBlend Blend              { get; set; }
        public List<IScenable> Components   { get; set; }
        public double RemainingTime         { get; private set; }

        private double length = 0;
					

        public Animation()
        {
            Length = 0;
            this.Paused = false;
            this.Position = Vector3.Zero;
            this.Blend = TypeBlend.Alpha;
            this.Components = null;
            this.VisualPriority = 0;
        }


        public Animation(double length)
        {
            Length = length;
            this.Paused = false;
            this.Position = Vector3.Zero;
            this.Blend = TypeBlend.Alpha;
            this.Components = null;
            this.VisualPriority = 0;
        }


        public double Length
        {
            get { return length; }
            set
            {
                RemainingTime += value - length;
                length = value;
            }
        }


        protected double RelativeTime
        {
            get { return this.Length - this.RemainingTime; }
        }


        public virtual void Initialize()
        {
            RemainingTime = Length;
        }


        public virtual void Update(GameTime gameTime)
        {
            RemainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public virtual bool Finished(GameTime gameTime)
        {
            return RemainingTime <= 0;
        }


        public virtual void Stop()
        {
            RemainingTime = 0;
        }


        public virtual void Draw(SpriteBatch spriteBatch) {}
    }
}

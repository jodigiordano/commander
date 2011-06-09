namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;


    public abstract class Animation
    {
        public bool Paused                  { get; set; }
        public double VisualPriority        { get; set; }
        public Scene Scene                  { protected get; set; }
        public double RemainingTime         { get; private set; }

        private double length = 0;


        public Animation(double visualPriority) : this(0, visualPriority) { }


        public Animation(double length, double visualPriority)
        {
            Length = length;
            this.Paused = false;
            this.VisualPriority = visualPriority;
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


        public virtual void Start()
        {
            RemainingTime = Length;

            Show();
        }


        public virtual void Stop()
        {
            RemainingTime = 0;

            Hide();
        }


        protected abstract void Show();
        protected abstract void Hide();


        public virtual void Draw() { }
    }
}

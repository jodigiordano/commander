namespace EphemereGames.Core.Visual
{
    using Microsoft.Xna.Framework;


    public abstract class Animation
    {
        public bool Paused                  { get; set; }
        public double VisualPriority        { get; set; }
        public Scene Scene                  { protected get; set; }
        public double RemainingTime         { get; private set; }

        private double length = 0;


        public Animation(double length, double visualPriority)
        {
            Length = length;
            Paused = false;
            VisualPriority = visualPriority;
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
            get { return Length - RemainingTime; }
        }


        public virtual void Initialize()
        {
            RemainingTime = Length;
        }


        public virtual void Update(GameTime gameTime)
        {
            RemainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public virtual bool IsFinished
        {
            get { return RemainingTime <= 0; }
        }


        public virtual void Start()
        {
            RemainingTime = Length;
        }


        public virtual void Stop()
        {
            RemainingTime = 0;
        }


        public virtual void Draw() { }
    }
}

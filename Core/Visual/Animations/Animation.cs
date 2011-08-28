namespace EphemereGames.Core.Visual
{
    using Microsoft.Xna.Framework;


    public abstract class Animation
    {
        public bool Paused                  { get; set; }
        public double VisualPriority        { get; set; }
        public Scene Scene                  { protected get; set; }
        public double RemainingTime         { get; private set; }

        private double duration = 0;


        public Animation(double duration, double visualPriority)
        {
            Duration = duration;
            Paused = false;
            VisualPriority = visualPriority;
        }


        public double Duration
        {
            get { return duration; }
            set
            {
                RemainingTime += value - duration;
                duration = value;
            }
        }


        protected double ElapsedTime
        {
            get { return Duration - RemainingTime; }
        }


        public virtual void Initialize()
        {
            RemainingTime = Duration;
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
            RemainingTime = Duration;
        }


        public virtual void Stop()
        {
            RemainingTime = 0;
        }


        public virtual void Draw() { }
    }
}

namespace EphemereGames.Core.Utilities
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    public abstract class AbstractEffect : ICloneable
    {
        public enum ProgressType
        {
            Linear,
            Now,
            After
        }


        [ContentSerializer(Optional = true)]
        public double Length { get; set; }

        [ContentSerializer(Optional = true)]
        public ProgressType Progress { get; set; }

        [ContentSerializer(Optional = true)]
        public double Delay { get; set; }

        [ContentSerializerIgnore]
        public bool Finished { get; set; }

        [ContentSerializerIgnore]
        public object Obj { get; set; }

        private double RemainingBeforeStart;
        private double RemainingBeforeEnd;
        protected double ElaspedTime;
        protected double TimeOneTick;
        private bool Initialized;


        public AbstractEffect()
        {
            Progress = ProgressType.Linear;
            Delay = 0;
            Length = 0;

            Initialize();
        }


        public void Update(GameTime gameTime)
        {
            if (!Initialized)
            {
                Initialize();
                InitializeLogic();

                Initialized = true;
            }

            Finished = (RemainingBeforeEnd <= 0 || gameTime == null);

            if (gameTime != null)
            {
                if (RemainingBeforeStart > 0)
                {
                    RemainingBeforeStart -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    return;
                }
                else
                {
                    ElaspedTime = Length - RemainingBeforeEnd;
                    RemainingBeforeEnd -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                TimeOneTick = gameTime.ElapsedGameTime.TotalMilliseconds;
            }


            if (!Finished && Progress == ProgressType.Now)
                LogicNow();

            else if (!Finished && Progress == ProgressType.Linear)
                LogicLinear();

            else if (Finished && Progress == ProgressType.After)
                LogicAfter();

            if (Finished)
                LogicEnd();
        }


        protected virtual void InitializeLogic() { }
        protected virtual void LogicLinear() { }
        protected virtual void LogicAfter() { }
        protected virtual void LogicNow() { }
        protected virtual void LogicEnd() { }


        public void Initialize()
        {
            RemainingBeforeEnd = Length;
            RemainingBeforeStart = Delay;
            Finished = false;
            Initialized = false;
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}


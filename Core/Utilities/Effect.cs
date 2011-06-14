namespace EphemereGames.Core.Utilities
{
    using System;
    using Microsoft.Xna.Framework;


    public abstract class Effect<T> : IEquatable<Effect<T>>
    {
        public enum ProgressType
        {
            Linear,
            Now,
            Logarithmic,
            After
        }


        public double Length;
        public ProgressType Progress;
        public double Delay;
        public bool Terminated;
        public T Obj                        { get; set; }
        public int Id;

        public NoneHandler TerminatedCallback;

        private double RemainingBeforeStart;
        private double RemainingBeforeEnd;
        protected double ElaspedTime;
        protected double TimeOneTick;
        private bool Initialized;

        private static int NextId = 0;


        public Effect()
        {
            Progress = ProgressType.Linear;
            Delay = 0;
            Length = 0;
            Id = NextId++;

            Initialize();
        }


        internal void Initialize()
        {
            ElaspedTime = 0;
            RemainingBeforeEnd = Length;
            RemainingBeforeStart = Delay;
            Terminated = false;
            Initialized = false;
        }


        internal void Update(GameTime gameTime)
        {
            if (!Initialized)
                return;

            Terminated = (RemainingBeforeEnd <= 0 || gameTime == null);

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
        }


        internal void UpdateObj(GameTime gameTime)
        {
            if (!Initialized)
            {
                InitializeLogic();
                Initialized = true;

                return;
            }

            if (!Terminated && Progress == ProgressType.Now)
                LogicNow();

            else if (!Terminated && Progress == ProgressType.Linear)
                LogicLinear();

            else if (!Terminated && Progress == ProgressType.Logarithmic)
                LogicLogarithmic();

            else if (Terminated && Progress == ProgressType.After)
                LogicAfter();

            if (Terminated)
                LogicEnd();
        }


        public bool Equals(Effect<T> other)
        {
            return Id == other.Id;
        }


        protected virtual void InitializeLogic() { }
        protected virtual void LogicLinear() { }
        protected virtual void LogicLogarithmic() { }
        protected virtual void LogicAfter() { }
        protected virtual void LogicNow() { }
        protected virtual void LogicEnd() { }


        internal abstract void Return();
    }
}


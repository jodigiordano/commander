namespace EphemereGames.Core.Utilities
{
    using System;


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
        public bool TerminatedOverride;
        public T Obj                        { get; set; }
        public int Id;

        public IntegerHandler TerminatedCallback;

        private double RemainingBeforeStart;
        protected double RemainingBeforeEnd;
        protected double ElaspedTime;
        protected double TimeOneTick;
        private bool Initialized;


        public Effect()
        {
            Progress = ProgressType.Linear;
            Delay = 0;
            Length = 0;

            Initialize();
        }


        internal void Initialize()
        {
            Id = Ids.NextEffectId++;
            ElaspedTime = 0;
            RemainingBeforeEnd = Length;
            RemainingBeforeStart = Delay;
            Terminated = false;
            TerminatedOverride = false;
            Initialized = false;
        }


        internal void Update(float elapsedTime)
        {
            if (RemainingBeforeStart > 0)
            {
                RemainingBeforeStart -= elapsedTime;
                return;
            }

            if (!Initialized)
                return;

            if (TerminatedOverride)
                return;

            Terminated = RemainingBeforeEnd <= 0;

            ElaspedTime = Length - RemainingBeforeEnd;
            RemainingBeforeEnd -= elapsedTime;

            TimeOneTick = elapsedTime;
        }


        internal void UpdateObj()
        {
            if (RemainingBeforeStart > 0)
                return;

            if (!Initialized)
            {
                InitializeLogic();
                Initialized = true;

                return;
            }

            if (TerminatedOverride)
                return;

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


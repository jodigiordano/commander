namespace EphemereGames.Core.Utilities
{
    public class PathEffect
    {
        public float Value;

        private Path Path;
        private double Duration;
        private double TickDuration;

        private double Elapsed;


        public PathEffect(Path path, double duration, double tickDuration)
        {
            Path = path;
            Duration = duration;
            TickDuration = tickDuration;
        }


        public bool Terminated
        {
            get { return Elapsed >= Duration; }
        }


        public void Initialize()
        {
            Elapsed = 0;
        }


        public void Update()
        {
            if (Terminated)
                return;

            Value = Path.GetPosition((float) Elapsed);

            Elapsed += TickDuration;
        }
    }
}

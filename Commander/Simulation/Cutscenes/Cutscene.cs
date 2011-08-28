namespace EphemereGames.Commander.Cutscenes
{
    abstract class Cutscene
    {
        public CommanderScene Scene;
        public bool Terminated = false;

        public abstract void Initialize();
        public abstract void Update();
        public abstract void Draw();
        public abstract void Stop();
    }
}

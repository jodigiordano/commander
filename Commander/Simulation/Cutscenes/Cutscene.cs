namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Core.Visual;


    abstract class Cutscene
    {
        public Scene Scene;
        public bool Terminated = false;

        public abstract void Initialize();
        public abstract void Update();
        public abstract void Draw();
        public abstract void Stop();
    }
}

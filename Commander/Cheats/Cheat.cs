namespace EphemereGames.Commander
{
    abstract class Cheat
    {
        public abstract bool Active     { get; }
        public bool ActivatedThisTick   { get; private set; }
        public string Name;
        protected bool Recurrent;


        public Cheat(string name)
        {
            Name = name;
            Recurrent = false;
        }


        public virtual void Initialize()
        {

        }


        public virtual void Update()
        {
            ActivatedThisTick = false;
        }


        protected void SetActivatedThisTick()
        {
            ActivatedThisTick = true;
        }
    }
}

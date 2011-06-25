namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class ToggleContextualMenuChoice : TextContextualMenuChoice
    {
        private List<string> Names;
        protected int CurrentIndex;


        public ToggleContextualMenuChoice(List<string> names)
            : base(new Text(names[0], "Pixelite") { SizeX = 2 })
        {
            Names = names;
            CurrentIndex = 0;
        }


        public virtual void Previous()
        {
            CurrentIndex--;

            if (CurrentIndex < 0)
                CurrentIndex = Names.Count - 1;

            base.SetData(Names[CurrentIndex]);
        }


        public virtual void Next()
        {
            CurrentIndex++;

            if (CurrentIndex >= Names.Count)
                CurrentIndex = 0;

            base.SetData(Names[CurrentIndex]);
        }


        public virtual void SetChoice(int index)
        {
            CurrentIndex = index;

            base.SetData(Names[CurrentIndex]);
        }
    }
}

namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class GeneralPanel : VerticalPanel
    {
        private Simulator Simulator;

        private ChoicesHorizontalSlider Difficulty;
        private NumericHorizontalSlider World;
        private NumericHorizontalSlider Level;


        public GeneralPanel(Simulator simulator, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(simulator.Scene, position, size, visualPriority, color)
        {
            Simulator = simulator;

            SetTitle("General");

            Difficulty = new ChoicesHorizontalSlider("Difficulty", new List<string>() { "Easy", "Normal", "Hard" }, 0);
            World = new NumericHorizontalSlider("World #", 1, 20, 1, 1, 100);
            Level = new NumericHorizontalSlider("Level #", 1, 50, 1, 1, 100);

            AddWidget("Difficulty", Difficulty);
            AddWidget("World", World);
            AddWidget("Level", Level);

            Initialize();
        }


        public void Initialize()
        {
            Difficulty.Value = Simulator.LevelDescriptor.Infos.Difficulty;

            var worldLevel = Simulator.LevelDescriptor.Infos.Mission.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

            World.Value = int.Parse(worldLevel[0]);
            Level.Value = int.Parse(worldLevel[1]);
        }
    }
}

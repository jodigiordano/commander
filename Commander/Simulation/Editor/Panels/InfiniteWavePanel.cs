namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class InfiniteWavePanel : VerticalPanel
    {
        public List<EnemyType> Enemies;

        private Simulator Simulator;

        private CheckBox Enable;
        private PushButton EnemiesButton;


        public InfiniteWavePanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White)
        {
            Simulator = simulator;

            SetTitle("Waves");

            Enemies = new List<EnemyType>();

            Enable = new CheckBox("Enable");
            EnemiesButton = new PushButton(new Text("Enemies", "Pixelite") { SizeX = 2 }, 250);

            AddWidget("Enable", Enable);
            AddWidget("Enemies", EnemiesButton);

            Alpha = 0;
        }


        public override void Open()
        {
            base.Open();

            Enemies.Clear();

            Enable.Value = Simulator.Data.Level.InfiniteWaves != null;

            if (Enable.Value)
            {
                foreach (var e in Simulator.Data.Level.InfiniteWaves.Enemies)
                    Enemies.Add(e);
            }
        }


        public override void Close()
        {
            base.Close();

            if (Enable.Value)
                Simulator.Data.Level.InfiniteWaves = EditorLevelGenerator.GenerateInfiniteWave(Simulator, Enemies);
            else
                Simulator.Data.Level.InfiniteWaves = null;
        }
    }
}

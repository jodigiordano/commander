namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class InfiniteWavePanel : VerticalPanel
    {
        private List<EnemyType> Enemies;

        private Simulator Simulator;

        private CheckBox Enable;
        private PushButton EnemiesButton;


        public InfiniteWavePanel(Simulator simulator)
            : base(simulator.Scene, Vector3.Zero, new Vector2(400, 300), VisualPriorities.Default.EditorPanel, Color.White)
        {
            Simulator = simulator;

            SetTitle("Waves");

            Enemies = new List<EnemyType>();

            Enable = new CheckBox("Enable") { SpaceForLabel = 250 };
            EnemiesButton = new PushButton(new Text("Enemies", "Pixelite") { SizeX = 2 }, 250) { ClickHandler = DoEnemiesPanel };

            AddWidget("Enable", Enable);
            AddWidget("Enemies", EnemiesButton);

            Padding = new Vector2(20, 40);

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
                Simulator.Data.Level.InfiniteWaves = MultiverseLevelGenerator.GenerateInfiniteWave(Simulator, Enemies);
            else
                Simulator.Data.Level.InfiniteWaves = null;
        }


        private void DoEnemiesPanel(PanelWidget widget, Commander.Player player)
        {
            var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels["EditorEnemies"];
            enemiesAssets.PanelToOpenOnClose = Name;

            enemiesAssets.Enemies = Enemies;

            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelShowCommand(Simulator.Data.Players[player], "EditorEnemies")
                {
                    UsePosition = true,
                    Position = Position + new Vector3(Size, 0)
                });
        }
    }
}

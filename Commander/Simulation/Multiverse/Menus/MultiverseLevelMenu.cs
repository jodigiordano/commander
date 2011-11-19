namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class MultiverseLevelMenu : MultiverseContextualMenu
    {
        public MultiverseLevelMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Play", "Play!", 2, DoPlay),
                new EditorTextContextualMenuChoice("Highscores", "Highscores", 2, DoHighscores),
            };

            foreach (var c in choices)
                AddChoice(c);
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.MultiverseMode &&
                    Simulator.WorldMode &&
                    !Simulator.EditingMode &&
                    Owner.ActualSelection.CelestialBody is Planet &&
                    ((Planet) Owner.ActualSelection.CelestialBody).IsALevel &&
                    !((WorldScene) Simulator.Scene).GetGamePausedSelected(Owner.InnerPlayer);
            }

            set { base.Visible = value; }
        }


        private void DoPlay()
        {
            Main.CurrentWorld.StartNewGame(Owner.InnerPlayer);
        }


        private void DoHighscores()
        {
            var panel = (HighscoresPanel) Simulator.Data.Panels["Highscores"];
            panel.WorldId = Main.CurrentWorld.World.Id;
            panel.LevelId = Main.CurrentWorld.World.GetLevelVisualId(Main.CurrentWorld.GetSelectedLevel(Owner.InnerPlayer).Infos.Id);

            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "Highscores"));
        }
    }
}

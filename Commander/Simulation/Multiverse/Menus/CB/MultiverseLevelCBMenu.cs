namespace EphemereGames.Commander.Simulation
{
    class MultiverseLevelCBMenu : MultiverseCelestialBodyMenu
    {
        public MultiverseLevelCBMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoiceFirst(new EditorTextContextualMenuChoice("Highscores", "Highscores", 2, DoHighscores)
            { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseHighscores) });
            
            AddChoiceFirst(new EditorTextContextualMenuChoice("playtest", "playtest level", 2, DoPlaytestLevel)
            { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiversePlaytestLevel) });
            
            AddChoiceFirst(new EditorTextContextualMenuChoice("edit", "edit level", 2, DoEditLevel)
            { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseEditLevel) });
            
            AddChoice(new EditorTextContextualMenuChoice("CelestialBodyAssets", "Asset", 2, DoCelestialBodyAssets)
            { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseAssets) });
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.WorldMode &&
                    Owner.ActualSelection.CelestialBody is Planet &&
                    ((Planet) Owner.ActualSelection.CelestialBody).IsALevel;
            }

            set { base.Visible = value; }
        }


        private void DoEditLevel()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorEditLevelCommand(Owner));

            Main.CurrentWorld.World.EditingMode = true;
            Main.CurrentWorld.World.MultiverseMode = true;
            Main.CurrentWorld.DoSelectActionEditor(Owner.InnerPlayer);
        }


        private void DoPlaytestLevel()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPlaytestLevelCommand(Owner));

            Main.CurrentWorld.World.EditingMode = false;
            Main.CurrentWorld.World.MultiverseMode = true;
            Main.CurrentWorld.DoSelectActionEditor(Owner.InnerPlayer);
        }


        private void DoCelestialBodyAssets()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAssets", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoAttributes()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorPanelCBShowCommand(Owner, "EditorPlanetCBAttributes", Owner.ActualSelection.CelestialBody, Simulator));
        }


        protected override void DoRemove()
        {
            base.DoRemove();

            Main.CurrentWorld.RemoveLevel(Owner.ActualSelection.CelestialBody);
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

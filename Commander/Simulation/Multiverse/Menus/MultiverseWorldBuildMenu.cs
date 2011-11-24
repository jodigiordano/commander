namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class MultiverseWorldBuildMenu : MultiverseContextualMenu
    {
        public MultiverseWorldBuildMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add level", 2, DoAddLevel)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseAddLevel) },
                
                new EditorTextContextualMenuChoice("AddPinkHole", "Add warp", 2, DoAddWarp)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseAddWarp) },
                
                new EditorTextContextualMenuChoice("AddPlanet", "Add planet", 2, DoAddPlanet)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseAddPlanet) },
                
                new EditorTextContextualMenuChoice("Background", "Background", 2, DoBackground)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseBackgrounds) },
                
                new EditorTextContextualMenuChoice("Waves", "Waves", 2, DoWaves)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseWaves) },
                
                new EditorTextContextualMenuChoice("ChangeName", "change name", 2, DoChangeName)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseChangeName) },
                
                new EditorTextContextualMenuChoice("Save", "save", 2, DoSave)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseSave) },
                
                new EditorTextContextualMenuChoice("JumpToWorld", "jump to world", 2, DoJumpToWorld)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseJumpToWorld) }
            };

            foreach (var c in choices)
                AddChoice(c);

            Visible = false;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.WorldMode &&
                    Simulator.EditingMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }


        public override void OnClose()
        {
            base.OnClose();

            Visible = false;
        }


        private void DoAddLevel()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                MultiverseLevelGenerator.GeneratePlanetCB(Simulator, VisualPriorities.Default.CelestialBody));

            Simulator.MultiverseController.ExecuteCommand(command);

            Main.CurrentWorld.AddLevel(command.CelestialBody);
            Visible = false;
        }


        private void DoAddWarp()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                MultiverseLevelGenerator.GeneratePinkHoleCB(Simulator, VisualPriorities.Default.CelestialBody));

            Simulator.MultiverseController.ExecuteCommand(command);

            Main.CurrentWorld.AddWarp(command.CelestialBody);
            Visible = false;
        }


        private void DoBackground()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorBackground"));
            Visible = false;
        }


        private void DoWaves()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorInfiniteWaves"));
            Visible = false;
        }


        private void DoChangeName()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorWorldName"));
            Visible = false;
        }


        private void DoSave()
        {
            Simulator.Data.Level.SyncDescriptor();
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "EditorSaveWorld"));
            Visible = false;
        }


        private void DoJumpToWorld()
        {
            ((JumpToWorldPanel) Simulator.Data.Panels["JumpToWorld"]).From = "World";
            Simulator.MultiverseController.ExecuteCommand(new EditorPanelShowCommand(Owner, "JumpToWorld"));
            Visible = false;
        }


        private void DoAddPlanet()
        {
            var command = new EditorCelestialBodyAddCommand(Owner,
                MultiverseLevelGenerator.GeneratePlanetCB(Simulator, VisualPriorities.Default.CelestialBody));

            ((Planet) command.CelestialBody).IsALevel = false;

            Simulator.MultiverseController.ExecuteCommand(command);
            Visible = false;
        }
    }
}

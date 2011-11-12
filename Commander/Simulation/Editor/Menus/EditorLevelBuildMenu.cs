﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorLevelBuildMenu : EditorContextualMenu
    {
        public EditorLevelBuildMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("AddPlanet", "Add a planet", 2, new EditorCelestialBodyCommand("AddPlanet")),
                new EditorTextContextualMenuChoice("AddPinkHole", "Add a pink hole", 2, new EditorCelestialBodyCommand("AddPinkHole")),
                new EditorTextContextualMenuChoice("Background", "Background", 2, new EditorShowPanelCommand("EditorBackground")),
                new EditorTextContextualMenuChoice("Turrets", "Turrets", 2, new EditorShowPanelCommand("EditorTurrets")),
                new EditorTextContextualMenuChoice("PowerUps", "Power-ups", 2, new EditorShowPanelCommand("EditorPowerUps")),
                new EditorTextContextualMenuChoice("Player", "Player", 2, new EditorShowPanelCommand("EditorPlayer")),
                new EditorTextContextualMenuChoice("Waves", "Waves", 2, new EditorShowPanelCommand("EditorWaves")),
                new EditorTextContextualMenuChoice("Playtest", "Playtest", 2, new EditorSimpleCommand("Playtest")),
            };

            foreach (var c in choices)
                AddChoice(c);

            Visible = false;
        }


        protected override EditorCommand Selection
        {
            get { return ((EditorTextContextualMenuChoice) Choices[SelectedIndex]).Command; }
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.GameMode &&
                    Simulator.EditorEditingMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }
    }
}
﻿namespace EphemereGames.Commander.Simulation.Player
{
    class EditorWorldMenu : WorldMenu
    {
        public EditorWorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            AddChoice(new EditorTextContextualMenuChoice("EditLayout", "edit layout", 2, new EditorSimpleCommand("Edit")));
            AddChoice(new EditorTextContextualMenuChoice("ChangeName", "change name", 2, new EditorSimpleCommand("ChangeName")));

            Visible = false;
        }


        public EditorCommand Selection
        {
            get { return ((EditorTextContextualMenuChoice) Choices[SelectedIndex]).Command; }
        }


        public override void UpdateSelection()
        {
            Owner.ActualSelection.EditorWorldCommand = Visible ? Selection : null;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.EditorPlaytestingMode &&
                    Simulator.WorldMode &&
                    Owner.ActualSelection.CelestialBody == null;
            }

            set { base.Visible = value; }
        }
    }
}

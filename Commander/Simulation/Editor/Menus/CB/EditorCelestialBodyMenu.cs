namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    abstract class EditorCelestialBodyMenu : EditorContextualMenu
    {
        public EditorCelestialBodyMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Move", "Move", 2, DoMove),
                new EditorTextContextualMenuChoice("Rotate", "Rotate", 2, DoRotate),
                new EditorTextContextualMenuChoice("Trajectory", "Trajectory", 2, DoTrajectory),
                new EditorTextContextualMenuChoice("Remove", "Remove", 2, DoRemove),
                new EditorToggleContextualMenuChoice("Speed",
                    new List<string>() { "Speed: 0", "Speed: 1", "Speed: 2", "Speed: 3", "Speed: 4", "Speed: 5", "Speed: 6", "Speed: 7", "Speed: 8", "Speed: 9", "Speed: 10" },
                    2, DoSpeed),
                new EditorTextContextualMenuChoice("PushFirst", "Push first on path", 2, DoPushFirst),
                new EditorTextContextualMenuChoice("PushLast", "Push last on path", 2, DoPushLast),
                new EditorTextContextualMenuChoice("RemoveFromPath", "Remove from path", 2, DoRemoveFromPath),
                new EditorToggleContextualMenuChoice("Size",
                    new List<string>() { "Size: small", "Size: normal", "Size: big" },
                    2, DoSize)
            };

            foreach (var c in choices)
                AddChoice(c);
        }


        public override void OnOpen()
        {
            SelectedIndex = 0;

            var speed = (EditorToggleContextualMenuChoice) GetChoiceByName("Speed");

            for (int i = 0; i < EditorLevelGenerator.PossibleRotationTimes.Count; i++)
                if (Owner.ActualSelection.CelestialBody.Speed == EditorLevelGenerator.PossibleRotationTimes[i])
                {
                    speed.SetChoice(i);
                    break;
                }
            
            var size = (EditorToggleContextualMenuChoice) GetChoiceByName("Size");

            size.SetChoice(
                Owner.ActualSelection.CelestialBody.Size == Size.Small ? 0 :
                Owner.ActualSelection.CelestialBody.Size == Size.Normal ? 1 : 2);
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.EditorEditingMode &&
                    Owner.ActualSelection.CelestialBody != null;
            }
            set { base.Visible = value; }
        }


        private void DoMove()
        {
            Simulator.EditorController.ExecuteCommand(
                new EditorCelestialBodyMoveCommand(Owner));

            Visible = false;
        }


        private void DoSpeed()
        {
            var widget = (EditorToggleContextualMenuChoice) GetChoiceByName("Speed");
            var speed = EditorLevelGenerator.PossibleRotationTimes
                [(widget.CurrentIndex + 1) % EditorLevelGenerator.PossibleRotationTimes.Count];

            Simulator.EditorController.ExecuteCommand(
                new EditorCelestialBodySpeedCommand(Owner, speed));

            widget.Next();
        }


        private void DoRotate()
        {
            Simulator.EditorController.ExecuteCommand(new EditorCelestialBodyRotateCommand(Owner));

            Visible = false;
        }


        private void DoTrajectory()
        {
            Simulator.EditorController.ExecuteCommand(new EditorCelestialBodyTrajectoryCommand(Owner));

            Visible = false;
        }


        private void DoRemove()
        {
            var command = new EditorCelestialBodyRemoveCommand(Owner);

            Simulator.EditorController.ExecuteCommand(command);

            if (Simulator.WorldMode && command.CelestialBody is Planet)
                Main.CurrentWorld.RemoveLevel(command.CelestialBody);
        }


        private void DoPushFirst()
        {
            Simulator.EditorController.ExecuteCommand(new EditorCelestialBodyPushFirstCommand(Owner));
        }


        private void DoPushLast()
        {
            Simulator.EditorController.ExecuteCommand(new EditorCelestialBodyPushLastCommand(Owner));
        }


        private void DoRemoveFromPath()
        {
            Simulator.EditorController.ExecuteCommand(new EditorCelestialBodyRemoveFromPathCommand(Owner));
        }


        private void DoSize()
        {
            var widget = (EditorToggleContextualMenuChoice) GetChoiceByName("Size");

            widget.Next();

            var sizeIndex = widget.CurrentIndex % 3;
            var size = sizeIndex == 0 ? Size.Small : sizeIndex == 1 ? Size.Normal : Size.Big;
                
            Simulator.EditorController.ExecuteCommand(
                new EditorCelestialBodySizeCommand(Owner, size));
        }
    }
}

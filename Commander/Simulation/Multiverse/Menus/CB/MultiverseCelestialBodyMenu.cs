namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    abstract class MultiverseCelestialBodyMenu : MultiverseContextualMenu
    {
        public MultiverseCelestialBodyMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {
            var choices = new List<ContextualMenuChoice>()
            {
                new EditorTextContextualMenuChoice("Move", "Move", 2, DoMove)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseMovePlanet) },
                new EditorTextContextualMenuChoice("Rotate", "Rotate", 2, DoRotate)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseRotatePlanet) },
                new EditorTextContextualMenuChoice("Trajectory", "Trajectory", 2, DoTrajectory)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiversePlanetTrajectory) },
                new EditorTextContextualMenuChoice("Remove", "Remove", 2, DoRemove)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseRemovePlanet) },
                new EditorToggleContextualMenuChoice("Speed",
                    new List<string>() { "Speed: 0", "Speed: 1", "Speed: 2", "Speed: 3", "Speed: 4", "Speed: 5", "Speed: 6", "Speed: 7", "Speed: 8", "Speed: 9", "Speed: 10" },
                    2, DoSpeed)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseTogglePlanetSpeed) },
                new EditorTextContextualMenuChoice("PushFirst", "Push first on path", 2, DoPushFirst)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiversePushFirstOnPath) },
                new EditorTextContextualMenuChoice("PushLast", "Push last on path", 2, DoPushLast)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiversePushLastOnPath) },
                new EditorTextContextualMenuChoice("RemoveFromPath", "Remove from path", 2, DoRemoveFromPath)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseRemoveFromPath) },
                new EditorToggleContextualMenuChoice("Size",
                    new List<string>() { "Size: small", "Size: normal", "Size: big" },
                    2, DoSize)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseTogglePlanetSize) },
                new EditorTextContextualMenuChoice("Attributes", "Attributes", 2, DoAttributes)
                { HelpBarMessage = Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.MultiverseOpenAttributes) }
            };

            foreach (var c in choices)
                AddChoice(c);
        }


        public override void OnOpen()
        {
            //SelectedIndex = 0;

            var speed = (EditorToggleContextualMenuChoice) GetChoiceByName("Speed");

            for (int i = 0; i < MultiverseLevelGenerator.PossibleRotationTimes.Count; i++)
                if (Owner.ActualSelection.CelestialBody.Speed == MultiverseLevelGenerator.PossibleRotationTimes[i])
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
                    Simulator.MultiverseMode &&
                    Simulator.EditingMode &&
                    Owner.ActualSelection.CelestialBody != null;
            }
            set { base.Visible = value; }
        }


        private void DoMove()
        {
            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodyMoveCommand(Owner));

            Visible = false;
        }


        private void DoSpeed()
        {
            var widget = (EditorToggleContextualMenuChoice) GetChoiceByName("Speed");
            var speed = MultiverseLevelGenerator.PossibleRotationTimes
                [(widget.CurrentIndex + 1) % MultiverseLevelGenerator.PossibleRotationTimes.Count];

            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodySpeedCommand(Owner, speed));

            widget.Next();
        }


        private void DoRotate()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorCelestialBodyRotateCommand(Owner));

            Visible = false;
        }


        private void DoTrajectory()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorCelestialBodyTrajectoryCommand(Owner));

            Visible = false;
        }


        protected virtual void DoRemove()
        {
            var command = new EditorCelestialBodyRemoveCommand(Owner);

            Simulator.MultiverseController.ExecuteCommand(command);
        }


        private void DoPushFirst()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorCelestialBodyPushFirstCommand(Owner));
        }


        private void DoPushLast()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorCelestialBodyPushLastCommand(Owner));
        }


        private void DoRemoveFromPath()
        {
            Simulator.MultiverseController.ExecuteCommand(new EditorCelestialBodyRemoveFromPathCommand(Owner));
        }


        private void DoSize()
        {
            var widget = (EditorToggleContextualMenuChoice) GetChoiceByName("Size");

            widget.Next();

            var sizeIndex = widget.CurrentIndex % 3;
            var size = sizeIndex == 0 ? Size.Small : sizeIndex == 1 ? Size.Normal : Size.Big;
                
            Simulator.MultiverseController.ExecuteCommand(
                new EditorCelestialBodySizeCommand(Owner, size));
        }


        protected virtual void DoAttributes()
        {

        }
    }
}

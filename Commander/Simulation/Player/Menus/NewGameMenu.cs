namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class NewGameMenu : ContextualMenu
    {
        private SimPlayer Owner;


        public NewGameMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, new List<ContextualMenuChoice>(), 10)
        {
            Owner = owner;
        }


        public override void Initialize()
        {
            Choices.Clear();

            if (Main.PlayersController.CampaignData.CurrentWorld == 0)
                return;

            AddChoice(new TextContextualMenuChoice("continue", new Text("Continue (World " + Main.PlayersController.CampaignData.CurrentWorld + ")", @"Pixelite") { SizeX = 2 }));
            AddChoice(new TextContextualMenuChoice("new", new Text("New campaign", @"Pixelite") { SizeX = 2 }));

            foreach (var w in Main.WorldsFactory.CampaignWorlds.Values)
            {
                if (w.Unlocked)
                    AddChoice(new TextContextualMenuChoice("jumpto", new Text("Jump to World " + w.Id, @"Pixelite") { SizeX = 2 }));
            }
        }


        public int Selection
        {
            get { return SelectedIndex; }
        }


        public override void NextChoice()
        {
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, false));
            base.NextChoice();
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, true));
        }


        public override void PreviousChoice()
        {
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, false));
            base.PreviousChoice();
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, true));
        }


        public override void UpdateSelection()
        {
             Owner.ActualSelection.NewGameChoice = Visible ? Selection : -1;
        }


        public override bool Visible
        {
            get
            {
                return
                    base.Visible &&
                    Simulator.MainMenuMode &&
                    Choices.Count != 0 &&
                    Owner.ActualSelection.CelestialBody != null &&
                    WorldsFactory.IsCampaignCB(Owner.ActualSelection.CelestialBody);
            }

            set { base.Visible = value; }
        }


        public override List<KeyValuePair<string, PanelWidget>> GetHelpBarMessage()
        {
            return Choices.Count == 0 ?
                Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.StartNewCampaign) :
                Simulator.HelpBar.GetPredefinedMessage(Owner.InnerPlayer, HelpBarMessage.ToggleChoicesSelect);
        }
    }
}

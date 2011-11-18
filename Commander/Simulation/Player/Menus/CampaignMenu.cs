namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class CampaignMenu : CommanderContextualMenu
    {
        public CampaignMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner)
        {

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


        public override void OnOpen()
        {
            Owner.ActualSelection.NewGameChoice = Selection;
        }


        public override void OnClose()
        {
            Owner.ActualSelection.NewGameChoice = -1;
        }


        public override void NextChoice()
        {
            base.NextChoice();

            Owner.ActualSelection.NewGameChoice = Selection;
        }


        public override void PreviousChoice()
        {
            base.PreviousChoice();

            Owner.ActualSelection.NewGameChoice = Selection;
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

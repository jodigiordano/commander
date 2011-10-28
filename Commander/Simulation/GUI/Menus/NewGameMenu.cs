namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewGameMenu : ContextualMenu
    {
        public CelestialBody CelestialBody;
        public int NewGameChoice;

        private bool AlternateSelectedText;


        public NewGameMenu(Simulator simulator, double visualPriority, Color color)
            : base(simulator, visualPriority, color, new List<ContextualMenuChoice>(), 10)
        {
            AlternateSelectedText = color == Colors.Spaceship.Yellow;
        }


        public void Initialize()
        {
            Choices.Clear();

            if (Main.SaveGameController.PlayerSaveGame.LevelsFinishedCount == 0 && Main.SaveGameController.PlayerSaveGame.CurrentWorld == 0)
                return;

            AddChoice(new TextContextualMenuChoice("continue", new Text("Continue (World " + Main.SaveGameController.PlayerSaveGame.CurrentWorld + ")", @"Pixelite") { SizeX = 2 }));
            AddChoice(new TextContextualMenuChoice("new", new Text("New campaign", @"Pixelite") { SizeX = 2 }));

            foreach (var w in Main.LevelsFactory.Worlds.Values)
            {
                if (w.Unlocked)
                    AddChoice(new TextContextualMenuChoice("jumpto", new Text("Jump to World " + w.Id, @"Pixelite") { SizeX = 2 }));
            }
        }


        public override bool Visible
        {
            get
            {
                return
                    Choices.Count != 0 &&
                    CelestialBody != null &&
                    LevelsFactory.IsCampaignCB(CelestialBody);
            }

            set { base.Visible = value; }
        }


        public HelpBarMessage GetHelpBarMessage()
        {
            return Choices.Count == 0 ? HelpBarMessage.StartNewCampaign : HelpBarMessage.ToggleChoicesSelect;
        }


        public void Update()
        {
            //if (CelestialBody != null)
            //    Position = CelestialBody.Position;
        }


        public override void Draw()
        {
            var newIndex = Math.Min((int) NewGameChoice, Choices.Count - 1);

            if (AlternateSelectedText && Choices.Count > 0 && newIndex != SelectedIndex)
            {
                if (SelectedIndex >= 0)
                    ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Color.White);

                if (newIndex >= 0)
                    ((TextContextualMenuChoice) Choices[newIndex]).SetColor(Colors.Spaceship.Selected);
            }

            SelectedIndex = newIndex;

            base.Draw();
        }
    }
}

﻿namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewGameMenu : ContextualMenu
    {
        public CelestialBody CelestialBody;
        public NewGameChoice NewGameChoice;


        public NewGameMenu(Simulator simulator, double visualPriority, Color color)
            : base(simulator, visualPriority, color, new List<ContextualMenuChoice>(), 10)
        {

        }


        public void Initialize()
        {
            Choices.Clear();

            if (Main.SaveGameController.PlayerSaveGame.LevelsFinishedCount == 0 && Main.SaveGameController.PlayerSaveGame.CurrentWorld == 0)
                return;

            AddChoice(new TextContextualMenuChoice("continue", new Text("Continue (World " + Main.SaveGameController.PlayerSaveGame.CurrentWorld + ")", "Pixelite") { SizeX = 2 }));

            var maxWorld = Main.SaveGameController.PlayerSaveGame.LastUnlockedWorld;

            if (maxWorld > 1)
                for (int i = 1; i <= maxWorld; i++)
                    AddChoice(new TextContextualMenuChoice("jumpto", new Text("Jump to World " + i, "Pixelite") { SizeX = 2 }));

            AddChoice(new TextContextualMenuChoice("new", new Text("New campaign", "Pixelite") { SizeX = 2 }));
        }


        public override bool Visible
        {
            get
            {
                return
                    Choices.Count != 0 &&
                    CelestialBody != null &&
                    CelestialBody.Name == "save the world";
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
            SelectedIndex = Math.Min((int) NewGameChoice, Choices.Count - 1);

            base.Draw();
        }
    }
}

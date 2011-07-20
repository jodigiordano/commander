namespace EphemereGames.Commander.Simulation
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

            if (Main.PlayerSaveGame.LevelsFinishedCount == 0 && Main.PlayerSaveGame.CurrentWorld == 0)
                return;

            AddChoice(new TextContextualMenuChoice(new Text("Continue (World " + Main.PlayerSaveGame.CurrentWorld + ")", "Pixelite") { SizeX = 2 }));

            var maxWorld = Main.PlayerSaveGame.LastUnlockedWorld;

            if (maxWorld > 1)
                for (int i = 1; i <= maxWorld; i++)
                    AddChoice(new TextContextualMenuChoice(new Text("Jump to World " + i, "Pixelite") { SizeX = 2 }));

            AddChoice(new TextContextualMenuChoice(new Text("New game", "Pixelite") { SizeX = 2 }));
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


        public void Update()
        {
            if (CelestialBody != null)
                Position = CelestialBody.Position;
        }


        public override void Draw()
        {
            SelectedIndex = Math.Min((int) NewGameChoice, Choices.Count - 1);

            base.Draw();
        }
    }
}

﻿namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpAnimation : Animation
    {
        private Translator Translator;
        private string Message;


        public HelpAnimation(string message)
            : base(5000, VisualPriorities.Default.HelpMessage)
        {
            Message = message;
        }


        public override void Initialize()
        {
            base.Initialize();

            Translator = new Translator(
                Scene,
                new Vector3(0, -250, 0),
                "Alien",
                Colors.Default.AlienBright,
                "Pixelite",
                Colors.Default.NeutralBright,
                Message,
                3,
                true,
                3000,
                100,
                VisualPriorities.Default.HelpMessage,
                true);
            Translator.CenterText = true;

            Scene.VisualEffects.Add(Translator, VisualEffects.FadeOutTo0(255, 4500, 500));
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Translator.Update();
        }


        public override void Draw()
        {
            base.Draw();

            Translator.Draw();
        }
    }
}
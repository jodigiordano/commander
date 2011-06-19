namespace EphemereGames.Commander.Simulation
{
    using Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LevelStartedAnnunciation
    {
        private Translator TranslatorMission;
        private Simulator Simulator;
        private double Term = 23000;
        private EffectsController<IVisual> EffectsController; // needed to be controller by this object because of the Tutorial system


        public LevelStartedAnnunciation(Simulator simulator, Level level)
        {
            Simulator = simulator;

            TranslatorMission = new Translator
            (Simulator.Scene, new Vector3(-600, -330, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", new Color(255, 255, 255, 0), level.Mission, 4, true, 4000, 250, Preferences.PrioriteGUIHistoire);

            EffectsController = new EffectsController<IVisual>();

            EffectsController.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 1000, 500));
            EffectsController.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 1000, 500));

            EffectsController.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 10000, 2000));
            EffectsController.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 10000, 2000));
        }

        public void Update(GameTime gameTime)
        {
            Term -= Preferences.TargetElapsedTimeMs;

            if (Term < 0)
                return;

            TranslatorMission.Update();
            EffectsController.Update(gameTime);
        }


        public bool Finished
        {
            get { return Term < 0; }
        }


        //public void Show()
        //{
        //    TranslatorMission.Show();
        //}


        //public void Hide()
        //{
        //    TranslatorMission.Hide();
        //}


        public void Draw()
        {
            if (Finished)
                return;

            TranslatorMission.Draw();
        }
    }
}

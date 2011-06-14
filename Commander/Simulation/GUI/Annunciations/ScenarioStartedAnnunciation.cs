namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;


    class ScenarioStartedAnnunciation
    {
        private Translator TranslatorMission;
        private Simulation Simulation;
        private double Term = 23000;
        private EffectsController EffectsController; // needed to be controller by this object because of the Tutorial system


        public ScenarioStartedAnnunciation(Simulation simulation, Scenario scenario)
        {
            Simulation = simulation;

            TranslatorMission = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -330, 0),
                "Alien",
                new Color(234, 196, 28, 0),
                "Pixelite",
                new Color(255, 255, 255, 0),
                scenario.Mission,
                4,
                true,
                4000,
                250,
                Preferences.PrioriteGUIHistoire
            );

            EffectsController = new EffectsController();

            EffectsController.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visual.PredefinedEffects.FadeInFrom0(255, 1000, 500));
            EffectsController.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visual.PredefinedEffects.FadeInFrom0(255, 1000, 500));

            EffectsController.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visual.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
            EffectsController.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visual.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
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

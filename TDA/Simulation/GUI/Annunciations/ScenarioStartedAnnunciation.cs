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


        public ScenarioStartedAnnunciation(Simulation simulation, Scenario scenario)
        {
            Simulation = simulation;

            TranslatorMission = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -330, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(255, 255, 255, 0),
                scenario.Mission,
                4,
                true,
                4000,
                250,
                Preferences.PrioriteGUIHistoire
            );

            Simulation.Scene.Effects.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 1000, 500));
            Simulation.Scene.Effects.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 1000, 500));

            Simulation.Scene.Effects.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
            Simulation.Scene.Effects.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
        }

        public void Update()
        {
            Term -= 16.66;

            if (Term < 0)
                return;

            TranslatorMission.Update();
        }


        public bool Finished
        {
            get { return Term < 0; }
        }


        public void Show()
        {
            TranslatorMission.Show();
        }


        public void Hide()
        {
            TranslatorMission.Hide();
        }
    }
}

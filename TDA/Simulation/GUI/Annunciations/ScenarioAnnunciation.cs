namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class ScenarioAnnunciation
    {
        private Translator TranslatorMission;
        private Translator TranslatorYearPlace;
        private Translator TranslatorObjective;
        private Simulation Simulation;
        private double Term = 23000;
        private EffectsController EffectsController;


        public ScenarioAnnunciation(Simulation simulation, Scenario scenario)
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

            TranslatorYearPlace = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -295, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                new Color(255, 255, 255, 0),
                scenario.Lieu,
                4,
                true,
                4000,
                250,
                Preferences.PrioriteGUIHistoire
            );

            //TranslatorObjective = new Translator
            //(
            //    Simulation.Main,
            //    Simulation.Scene,
            //    new Vector3(-600, -260, 0),
            //    EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Alien"),
            //    new Color(234, 196, 28, 0),
            //    EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
            //    new Color(255, 255, 255, 0),
            //    scenario.Objectif,
            //    4,
            //    true,
            //    5000,
            //    100,
            //    Preferences.PrioriteGUIHistoire
            //);

            EffectsController = new EffectsController();

            EffectsController.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 1000, 500));
            EffectsController.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 1000, 500));

            EffectsController.Add(TranslatorYearPlace.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 2000, 250));
            EffectsController.Add(TranslatorYearPlace.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 2000, 250));

            //EffectsController.Add(TranslatorObjective.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 3000, 100));
            //EffectsController.Add(TranslatorObjective.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeInFrom0(255, 3000, 100));

            EffectsController.Add(TranslatorMission.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
            EffectsController.Add(TranslatorMission.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));

            EffectsController.Add(TranslatorYearPlace.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
            EffectsController.Add(TranslatorYearPlace.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));

            //EffectsController.Add(TranslatorObjective.PartieNonTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
            //EffectsController.Add(TranslatorObjective.PartieTraduite, EphemereGames.Core.Visuel.PredefinedEffects.FadeOutTo0(255, 10000, 2000));
        }

        public void Update(GameTime gameTime)
        {
            Term -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Term < 0)
                return;

            TranslatorMission.Update(gameTime);
            TranslatorYearPlace.Update(gameTime);
            //TranslatorObjective.Update(gameTime);
            EffectsController.Update(gameTime);
        }

        public void Draw()
        {
            if (Term < 0)
                return;

            TranslatorMission.Draw(null);
            TranslatorYearPlace.Draw(null);
            //TranslatorObjective.Draw(null);
        }
    }
}

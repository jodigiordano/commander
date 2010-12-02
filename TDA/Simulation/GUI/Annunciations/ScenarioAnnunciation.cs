namespace TDA
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class ScenarioAnnunciation
    {
        private Translator TranslatorMission;
        private Translator TranslatorYearPlace;
        private Translator TranslatorObjective;
        private Simulation Simulation;
        private double Term = 23000;

        public ScenarioAnnunciation(Simulation simulation, Scenario scenario)
        {
            Simulation = simulation;

            TranslatorMission = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -330, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 0),
                scenario.Mission,
                4,
                true,
                4000,
                250
            );
            TranslatorMission.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;
            TranslatorMission.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;

            TranslatorYearPlace = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -295, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 0),
                scenario.Lieu,
                4,
                true,
                4000,
                250
            );
            TranslatorYearPlace.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;
            TranslatorYearPlace.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;

            TranslatorObjective = new Translator
            (
                Simulation.Main,
                Simulation.Scene,
                new Vector3(-600, -260, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 0),
                scenario.Objectif,
                4,
                true,
                5000,
                100
            );
            TranslatorObjective.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;
            TranslatorObjective.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;

            Simulation.Scene.Effets.ajouter(TranslatorMission.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 1000, 500));
            Simulation.Scene.Effets.ajouter(TranslatorMission.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 1000, 500));

            Simulation.Scene.Effets.ajouter(TranslatorYearPlace.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 2000, 250));
            Simulation.Scene.Effets.ajouter(TranslatorYearPlace.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 2000, 250));

            Simulation.Scene.Effets.ajouter(TranslatorObjective.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 3000, 100));
            Simulation.Scene.Effets.ajouter(TranslatorObjective.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 3000, 100));


            Simulation.Scene.Effets.ajouter(TranslatorMission.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
            Simulation.Scene.Effets.ajouter(TranslatorMission.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));

            Simulation.Scene.Effets.ajouter(TranslatorYearPlace.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
            Simulation.Scene.Effets.ajouter(TranslatorYearPlace.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));

            Simulation.Scene.Effets.ajouter(TranslatorObjective.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
            Simulation.Scene.Effets.ajouter(TranslatorObjective.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
        }

        public void Update(GameTime gameTime)
        {
            Term -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Term < 0)
                return;

            TranslatorMission.Update(gameTime);
            TranslatorYearPlace.Update(gameTime);
            TranslatorObjective.Update(gameTime);
        }

        public void Draw()
        {
            if (Term < 0)
                return;

            TranslatorMission.Draw(null);
            TranslatorYearPlace.Draw(null);
            TranslatorObjective.Draw(null);
        }
    }
}

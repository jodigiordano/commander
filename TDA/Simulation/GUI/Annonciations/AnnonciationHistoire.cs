namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;

    class AnnonciationHistoire : DrawableGameComponent
    {
        private Traducteur TraductionMission;
        private Traducteur TraductionAnneeLieu;
        private Traducteur TraductionObjectif;

        private Scene Scene;

        private double Duree = 23000;

        public AnnonciationHistoire(Simulation simulation, Scenario histoire)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;

            TraductionMission = new Traducteur
            (
                simulation.Main,
                simulation.Scene,
                new Vector3(-600, -330, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 0),
                histoire.Mission,
                4,
                true,
                4000,
                250
            );
            TraductionMission.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;
            TraductionMission.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;

            TraductionAnneeLieu = new Traducteur
            (
                simulation.Main,
                simulation.Scene,
                new Vector3(-600, -295, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 0),
                histoire.Lieu,
                4,
                true,
                4000,
                250
            );
            TraductionAnneeLieu.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;
            TraductionAnneeLieu.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;

            TraductionObjectif = new Traducteur
            (
                simulation.Main,
                simulation.Scene,
                new Vector3(-600, -260, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Alien"),
                new Color(234, 196, 28, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                new Color(Color.White, 0),
                histoire.Objectif,
                4,
                true,
                5000,
                100
            );
            TraductionObjectif.PartieTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;
            TraductionObjectif.PartieNonTraduite.PrioriteAffichage = Preferences.PrioriteGUIHistoire;

            Scene.Effets.ajouter(TraductionMission.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 1000, 500));
            Scene.Effets.ajouter(TraductionMission.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 1000, 500));

            Scene.Effets.ajouter(TraductionAnneeLieu.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 2000, 250));
            Scene.Effets.ajouter(TraductionAnneeLieu.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 2000, 250));

            Scene.Effets.ajouter(TraductionObjectif.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 3000, 100));
            Scene.Effets.ajouter(TraductionObjectif.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeInFrom0(255, 3000, 100));


            Scene.Effets.ajouter(TraductionMission.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
            Scene.Effets.ajouter(TraductionMission.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));

            Scene.Effets.ajouter(TraductionAnneeLieu.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
            Scene.Effets.ajouter(TraductionAnneeLieu.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));

            Scene.Effets.ajouter(TraductionObjectif.PartieNonTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
            Scene.Effets.ajouter(TraductionObjectif.PartieTraduite, Core.Visuel.EffetsPredefinis.fadeOutTo0(255, 10000, 2000));
        }

        public override void Update(GameTime gameTime)
        {
            Duree -= gameTime.ElapsedGameTime.TotalMilliseconds;

            TraductionMission.Update(gameTime);
            TraductionAnneeLieu.Update(gameTime);
            TraductionObjectif.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Duree > 0)
            {
                TraductionMission.Draw(gameTime);
                TraductionAnneeLieu.Draw(gameTime);
                TraductionObjectif.Draw(gameTime);
            }
        }
    }
}

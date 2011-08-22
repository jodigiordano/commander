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
            (Simulator.Scene, new Vector3(-600, -330, 0), "Alien", Colors.Default.AlienBright, @"Pixelite", Colors.Default.NeutralBright, level.Mission, 4, true, 4000, 250, Preferences.PrioriteGUIHistoire, false);

            EffectsController = new EffectsController<IVisual>();

            EffectsController.Add(TranslatorMission.ToTranslate, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 1000, 500));
            EffectsController.Add(TranslatorMission.Translated, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 1000, 500));

            EffectsController.Add(TranslatorMission.ToTranslate, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 10000, 2000));
            EffectsController.Add(TranslatorMission.Translated, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(255, 10000, 2000));
        }

        public void Update()
        {
            Term -= Preferences.TargetElapsedTimeMs;

            if (Term < 0)
                return;

            TranslatorMission.Update();
            EffectsController.Update(Preferences.TargetElapsedTimeMs);
        }


        public bool Finished
        {
            get { return Term < 0; }
        }


        public void Draw()
        {
            if (Finished)
                return;

            TranslatorMission.Draw();
        }
    }
}

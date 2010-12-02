namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class GUIController
    {
        public Dictionary<TypeEnnemi, DescripteurEnnemi> CompositionNextWave;

        private Simulation Simulation;
        private SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        private MenuGeneral MenuGeneral;

        public Sablier SandGlass { get { return MenuGeneral.SandGlass; } }



        public GUIController(Simulation simulation)
        {
            Simulation = simulation;
            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulation);
            MenuGeneral = new MenuGeneral(Simulation, new Vector3(400, -260, 0));
        }


        public void Initialize()
        {
            MenuGeneral.CompositionNextWave = CompositionNextWave;
        }


        public void doSelectedCelestialBodyChanged(CorpsCeleste celestialBody)
        {
            SelectedCelestialBodyAnimation.CelestialBody = celestialBody;
        }


        public void doShowCompositionNextWave()
        {
            MenuGeneral.MenuNextWave.Visible = true;
        }


        public void doHideCompositionNextWave()
        {
            MenuGeneral.MenuNextWave.Visible = false;
        }


        public void doNextWave()
        {
            MenuGeneral.SandGlass.tourner();
        }


        public void doRemainingWavesChanged(int waves)
        {
            MenuGeneral.RemainingWaves = waves;
        }


        public void doTimeNextWaveChanged(double time)
        {
            MenuGeneral.TimeNextWave = time;
        }


        public void doScoreChanged(int score)
        {
            MenuGeneral.Score = score;
        }


        public void doCashChanged(int cash)
        {
            MenuGeneral.Cash = cash;
        }


        public void Update(GameTime gameTime)
        {
            SelectedCelestialBodyAnimation.Update(gameTime);

            if (!Simulation.ModeDemo)
            {
                MenuGeneral.Update(gameTime);
            }
        }


        public void Draw()
        {
            SelectedCelestialBodyAnimation.Draw();

            if (!Simulation.ModeDemo)
            {
                MenuGeneral.Draw();
            }
        }
    }
}

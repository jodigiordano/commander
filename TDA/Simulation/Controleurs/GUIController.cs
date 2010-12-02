namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;
    using System;

    class GUIController
    {
        public List<CorpsCeleste> CelestialBodies;
        public Dictionary<TypeEnnemi, DescripteurEnnemi> CompositionNextWave;
        public Scenario Scenario;
        public List<Ennemi> Enemies;
        public Cursor Cursor;
        public VaguesInfinies InfiniteWaves;
        public LinkedList<Vague> Waves;

        private Simulation Simulation;
        private SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        private MenuGeneral MenuGeneral;
        private ScenarioAnnunciation ScenarioAnnunciation;
        private ScenarioEndedAnnunciation ScenarioEndedAnnunciation;
        private AdvancedView AdvancedView;
        private PlayerLives PlayerLives;


        public GUIController(Simulation simulation)
        {
            Simulation = simulation;
            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulation);
            MenuGeneral = new MenuGeneral(Simulation, new Vector3(400, -260, 0));
            Cursor = new Cursor(Simulation.Main, Simulation.Scene, Vector3.Zero, 10, Preferences.PrioriteGUIPanneauGeneral);
        }


        public void Initialize()
        {
            MenuGeneral.CompositionNextWave = CompositionNextWave;
            ScenarioAnnunciation = new ScenarioAnnunciation(Simulation, Scenario);
            ScenarioEndedAnnunciation = new ScenarioEndedAnnunciation(Simulation, CelestialBodies);
            AdvancedView = new AdvancedView(Simulation, Enemies, CelestialBodies);
            PlayerLives = new PlayerLives(Simulation, Scenario.CorpsCelesteAProteger, new Color(255, 0, 220));

            MenuGeneral.RemainingWaves = (InfiniteWaves == null) ? Waves.Count : -1;
            MenuGeneral.TimeNextWave = Waves.First.Value.TempsApparition;
        }


        public Sablier SandGlass
        {
            get { return MenuGeneral.SandGlass; }
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


        public void doScoreChanged(int score)
        {
            MenuGeneral.Score = score;
        }


        public void doCashChanged(int cash)
        {
            MenuGeneral.Cash = cash;
        }


        public void doGameStateChanged(EtatPartie newGameState)
        {
            ScenarioEndedAnnunciation.doGameStateChanged(newGameState);
        }


        public void doShowAdvancedView()
        {
            AdvancedView.Visible = true;
        }


        public void doHideAdvancedView()
        {
            AdvancedView.Visible = false;
        }


        public void doObjectDestroyed(IObjetPhysique obj)
        {
            VaisseauDoItYourself vaisseau = obj as VaisseauDoItYourself;

            if (vaisseau != null)
            {
                Cursor.Position = vaisseau.Position;
                Cursor.doShow();

                return;
            }
        }


        public void doSpaceshipBuyed(CorpsCeleste celestialBody)
        {
            Cursor.doHide();
        }


        public void doWaveStarted()
        {
            MenuGeneral.TimeNextWave = double.MaxValue;
            MenuGeneral.RemainingWaves--;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxNouvelleVague");

            if (InfiniteWaves != null || MenuGeneral.RemainingWaves <= 0)
                return;

            //todo
            LinkedListNode<Vague> vagueSuivante = Waves.First;

            for (int i = 0; i < Waves.Count - MenuGeneral.RemainingWaves; i++)
                vagueSuivante = vagueSuivante.Next;

            MenuGeneral.TimeNextWave = vagueSuivante.Value.TempsApparition;
        }


        public void Update(GameTime gameTime)
        {
            if (MenuGeneral.TimeNextWave > 0)
                MenuGeneral.TimeNextWave = Math.Max(0, MenuGeneral.TimeNextWave - gameTime.ElapsedGameTime.TotalMilliseconds);

            SelectedCelestialBodyAnimation.Update(gameTime);

            //todo event-based
            MenuGeneral.MenuNextWave.Visible = Cursor.Actif && Core.Physique.Facade.collisionCercleRectangle(Cursor.Cercle, SandGlass.Rectangle);

            if (!Simulation.ModeDemo)
            {
                MenuGeneral.Update(gameTime);
                ScenarioAnnunciation.Update(gameTime);
                ScenarioEndedAnnunciation.Update(gameTime);
                PlayerLives.Update(gameTime);
            }
        }


        public void Draw()
        {
            SelectedCelestialBodyAnimation.Draw();
            Cursor.Draw();

            if (!Simulation.ModeDemo)
            {
                MenuGeneral.Draw();
                ScenarioAnnunciation.Draw();
                ScenarioEndedAnnunciation.Draw();
                AdvancedView.Draw();
                PlayerLives.Draw();
            }
        }
    }
}

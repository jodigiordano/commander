namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Visuel;
    using Core.Physique;


    enum GameState
    {
        Running,
        Won,
        Lost
    }


    class ScenarioController
    {
        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        public bool DemoMode = false;
        public bool EditorMode = false;

        public List<CorpsCeleste> CelestialBodies   { get { return Scenario.SystemePlanetaire; } }
        public VaguesInfinies InfiniteWaves         { get { return Scenario.VaguesInfinies; } }
        public LinkedList<Vague> Waves              { get { return Scenario.Vagues; } }
        public CommonStash CommonStash              { get { return Scenario.CommonStash; } }
        public List<Tourelle> StartingTurrets       { get { return Scenario.Tourelles; } }
        public CorpsCeleste CelestialBodyToProtect  { get { return Scenario.CorpsCelesteAProteger; } }

        public GameState State                      { get; private set; }
        private Simulation Simulation;
        private int WavesCounter;
        private double ParTime;
        private ParticuleEffectWrapper Stars;
        private double StarsEmitter;
        public Scenario Scenario;


        public ScenarioController(Simulation simulation, Scenario scenario)
        {
            Simulation = simulation;
            Scenario = scenario;

            Stars = Simulation.Scene.Particules.recuperer("etoilesScintillantes");
            Stars.PrioriteAffichage = Preferences.PrioriteGUIEtoiles;
            StarsEmitter = 0;

            WavesCounter = 0;

            ParTime = (Scenario.VaguesInfinies != null) ? 0 : Scenario.Vagues.Last.Value.TempsApparition + Scenario.Vagues.Last.Value.Ennemis.Count * 2000;
        }


        public void Update(GameTime gameTime)
        {
            StarsEmitter += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (StarsEmitter >= 100)
            {
                Vector2 v2 = Vector2.Zero;
                Stars.Emettre(ref v2);
                StarsEmitter = 0;
            }

            if (Main.Random.Next(0, 1000) == 0)
                Simulation.Scene.Animations.inserer(Simulation.Scene, new AnimationEtoileFilante(Simulation));

            ParTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public void Draw(GameTime gameTime)
        {
            Simulation.Scene.ajouterScenable(Scenario.FondEcran);
        }


        public void doWaveEnded()
        {
            WavesCounter++;

            if (WavesCounter == Waves.Count && State == GameState.Running && !DemoMode && !EditorMode)
            {
                State = GameState.Won;

                if (!Simulation.Main.SaveGame.Progress.ContainsKey(Scenario.Numero))
                    Simulation.Main.SaveGame.Progress.Add(Scenario.Numero, 0);

                if (Simulation.Main.SaveGame.Progress[Scenario.Numero] < 0)
                    Simulation.Main.SaveGame.Progress[Scenario.Numero] = Math.Abs(Simulation.Main.SaveGame.Progress[Scenario.Numero]);
                else if (Simulation.Main.SaveGame.Progress[Scenario.Numero] == 0)
                    Simulation.Main.SaveGame.Progress[Scenario.Numero] = 1;

                computeFinalScore();

                notifyNouvelEtatPartie(State);
            }
        }


        public void doEnemyReachedEnd(Ennemi enemy, CorpsCeleste celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!(celestialBody is CeintureAsteroide))
                celestialBody.doTouche(enemy);

            if (!Simulation.ModeDemo && Simulation.Etat != GameState.Lost)
            {
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteTouche");
            }

            if (celestialBody == CelestialBodyToProtect)
                CommonStash.Lives = (int) CelestialBodyToProtect.PointsVie; //correct de caster?

            if (CommonStash.Lives <= 0 && State == GameState.Running && !DemoMode && !EditorMode)
            {
                CelestialBodyToProtect.doMeurt();
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteExplose");
                State = GameState.Lost;

                if (!Simulation.Main.SaveGame.Progress.ContainsKey(Scenario.Numero))
                    Simulation.Main.SaveGame.Progress.Add(Scenario.Numero, 0);

                if ((Simulation.Main.SaveGame.Progress[Scenario.Numero] <= 0))
                    Simulation.Main.SaveGame.Progress[Scenario.Numero] -= 1;

                computeFinalScore();

                notifyNouvelEtatPartie(State);
            }
        }
        

        public void doObjectDestroyed(IObjetPhysique obj)
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;

            CorpsCeleste celestialBody = obj as CorpsCeleste;

            if (celestialBody == null)
                return;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteExplose");

            if (celestialBody == CelestialBodyToProtect && !DemoMode && !EditorMode)
            {
                State = GameState.Lost;

                if (!Simulation.Main.SaveGame.Progress.ContainsKey(Scenario.Numero))
                    Simulation.Main.SaveGame.Progress.Add(Scenario.Numero, 0);

                if ((Simulation.Main.SaveGame.Progress[Scenario.Numero] <= 0))
                    Simulation.Main.SaveGame.Progress[Scenario.Numero] -= 1;

                computeFinalScore();

                notifyNouvelEtatPartie(State);
            }
        }


        private void computeFinalScore()
        {
            ParTime = Math.Max(0, ParTime);

            Scenario.CommonStash.Score += Scenario.CommonStash.Lives * 50;
            Scenario.CommonStash.Score += Scenario.CommonStash.Cash;

            if (State == GameState.Won)
                Scenario.CommonStash.Score += (int) (ParTime / 100);

            notifyCommonStashChanged(Scenario.CommonStash);
        }


        private void notifyCommonStashChanged(CommonStash stash)
        {
            if (CommonStashChanged != null)
                CommonStashChanged(stash);
        }


        private void notifyNouvelEtatPartie(GameState nouvelEtat)
        {
            if (NewGameState != null)
                NewGameState(nouvelEtat);
        }
    }
}

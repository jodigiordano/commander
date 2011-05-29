namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    enum GameState
    {
        Running,
        Paused,
        Won,
        Lost
    }


    class ScenarioController
    {
        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        public bool DemoMode = false;
        public bool EditorMode = false;
        public Help Help;

        public List<CorpsCeleste> CelestialBodies                   { get { return Scenario.PlanetarySystem; } }
        public VaguesInfinies InfiniteWaves                         { get { return Scenario.InfiniteWaves; } }
        public LinkedList<Wave> Waves                               { get { return Scenario.Waves; } }
        public CommonStash CommonStash                              { get { return Scenario.CommonStash; } }
        public List<Turret> StartingTurrets                         { get { return Scenario.Turrets; } }
        public CorpsCeleste CelestialBodyToProtect                  { get { return Scenario.CelestialBodyToProtect; } }
        public Dictionary<TurretType, Turret> AvailableTurrets      { get { return Scenario.AvailableTurrets; } }
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps   { get { return Scenario.AvailablePowerUps; } }

        public GameState State;
        public Scenario Scenario;

        private Simulation Simulation;
        private int WavesCounter;
        private ParticuleEffectWrapper Stars;
        private double StarsEmitter;
        private double ElapsedTime;

        private bool HelpSaved;


        public ScenarioController(Simulation simulation, Scenario scenario)
        {
            Simulation = simulation;
            Scenario = scenario;

            Stars = Simulation.Scene.Particules.recuperer("etoilesScintillantes");
            Stars.VisualPriority = Preferences.PrioriteGUIEtoiles;
            StarsEmitter = 0;

            WavesCounter = 0;
            ElapsedTime = 0;

            if (Simulation.Main.SaveGame.Tutorials.ContainsKey(Scenario.Id) && Simulation.Main.SaveGame.Tutorials[Scenario.Id] > 2)
            {
                Help = new Help(Simulation, new List<string>());
                HelpSaved = true;
            }

            else
            {
                Help = new Help(simulation, scenario.HelpTexts);
                HelpSaved = false;
            }
        }


        public void Update(GameTime gameTime)
        {
            if (Help.Active)
                Help.Update(gameTime);
            else if (!HelpSaved)
            {
                if (!Simulation.Main.SaveGame.Tutorials.ContainsKey(Scenario.Id))
                    Simulation.Main.SaveGame.Tutorials.Add(new KeyValuePair<int,int>(Scenario.Id, 0));

                Simulation.Main.SaveGame.Tutorials[Scenario.Id]++;

                HelpSaved = true;
            }


            StarsEmitter += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (StarsEmitter >= 100)
            {
                Vector2 v2 = Vector2.Zero;
                Stars.Emettre(ref v2);
                StarsEmitter = 0;
            }

            if (Main.Random.Next(0, 1000) == 0)
                Simulation.Scene.Animations.Insert(new AnimationEtoileFilante(Simulation));

            ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public void TriggerNewGameState(GameState state)
        {
            notifyNouvelEtatPartie(state);
        }


        public void Draw(GameTime gameTime)
        {
            Simulation.Scene.ajouterScenable(Scenario.Background);

            if (Help.Active)
                Help.Draw();
        }


        public void doWaveEnded()
        {
            WavesCounter++;

            if (WavesCounter == Waves.Count && State == GameState.Running && !DemoMode && !EditorMode)
            {
                State = GameState.Won;

                if (!Simulation.Main.SaveGame.Progress.ContainsKey(Scenario.Id))
                    Simulation.Main.SaveGame.Progress.Add(Scenario.Id, 0);

                if (Simulation.Main.SaveGame.Progress[Scenario.Id] < 0)
                    Simulation.Main.SaveGame.Progress[Scenario.Id] = Math.Abs(Simulation.Main.SaveGame.Progress[Scenario.Id]);
                else if (Simulation.Main.SaveGame.Progress[Scenario.Id] == 0)
                    Simulation.Main.SaveGame.Progress[Scenario.Id] = 1;

                computeFinalScore();

                notifyNouvelEtatPartie(State);
            }
        }


        public void doEnemyReachedEnd(Ennemi enemy, CorpsCeleste celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!(celestialBody is AsteroidBelt))
                celestialBody.DoHit(enemy);

            if (!Simulation.ModeDemo && Simulation.Etat != GameState.Lost)
            {
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteTouche");
            }

            if (celestialBody == CelestialBodyToProtect)
                CommonStash.Lives = (int) CelestialBodyToProtect.LifePoints; //correct de caster?

            if (CommonStash.Lives <= 0 && State == GameState.Running && !DemoMode && !EditorMode)
            {
                CelestialBodyToProtect.DoDie();
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteExplose");
                State = GameState.Lost;

                if (!Simulation.Main.SaveGame.Progress.ContainsKey(Scenario.Id))
                    Simulation.Main.SaveGame.Progress.Add(Scenario.Id, 0);

                if ((Simulation.Main.SaveGame.Progress[Scenario.Id] <= 0))
                    Simulation.Main.SaveGame.Progress[Scenario.Id] -= 1;

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

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxCorpsCelesteExplose");

            if (celestialBody == CelestialBodyToProtect && !DemoMode && !EditorMode)
            {
                State = GameState.Lost;

                if (!Simulation.Main.SaveGame.Progress.ContainsKey(Scenario.Id))
                    Simulation.Main.SaveGame.Progress.Add(Scenario.Id, 0);

                if ((Simulation.Main.SaveGame.Progress[Scenario.Id] <= 0))
                    Simulation.Main.SaveGame.Progress[Scenario.Id] -= 1;

                computeFinalScore();

                notifyNouvelEtatPartie(State);
            }
        }


        private void computeFinalScore()
        {
            ElapsedTime = Math.Min(Scenario.ParTime, ElapsedTime);

            Scenario.CommonStash.TotalScore += Scenario.CommonStash.Lives * 50;
            Scenario.CommonStash.TotalScore += Scenario.CommonStash.Cash;

            if (State == GameState.Won)
            {
                Scenario.CommonStash.TimeLeft = (int)((Scenario.ParTime - ElapsedTime) / 100);
                Scenario.CommonStash.TotalScore += Scenario.CommonStash.TimeLeft;
            }

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

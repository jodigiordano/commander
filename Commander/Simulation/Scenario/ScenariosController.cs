namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    enum GameState
    {
        Running,
        Paused,
        Won,
        Lost
    }


    class ScenariosController
    {
        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        public bool EditorMode = false;
        public Help Help;

        public List<CelestialBody> CelestialBodies                   { get { return Scenario.PlanetarySystem; } }
        public InfiniteWave InfiniteWaves                         { get { return Scenario.InfiniteWaves; } }
        public LinkedList<Wave> Waves                               { get { return Scenario.Waves; } }
        public CommonStash CommonStash                              { get { return Scenario.CommonStash; } }
        public List<Turret> StartingTurrets                         { get { return Scenario.Turrets; } }
        public CelestialBody CelestialBodyToProtect                  { get { return Scenario.CelestialBodyToProtect; } }
        public Dictionary<TurretType, Turret> AvailableTurrets      { get { return Scenario.AvailableTurrets; } }
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps   { get { return Scenario.AvailablePowerUps; } }

        public GameState State;
        public Scenario Scenario;

        private Simulation Simulation;
        private int WavesCounter;
        private double ElapsedTime;

        private bool HelpSaved;


        public ScenariosController(Simulation simulation)
        {
            Simulation = simulation;
        }


        public void Initialize()
        {
            WavesCounter = 0;
            ElapsedTime = 0;

            if (Simulation.Main.SaveGame.Tutorials.ContainsKey(Scenario.Id) && Simulation.Main.SaveGame.Tutorials[Scenario.Id] > 2)
            {
                Help = new Help(Simulation, new List<string>());
                HelpSaved = true;
            }

            else
            {
                Help = new Help(Simulation, Scenario.HelpTexts);
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

            ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public void TriggerNewGameState(GameState state)
        {
            notifyNouvelEtatPartie(state);
        }


        //public void Show()
        //{
        //    Simulation.Scene.Add(Scenario.Background);

        //    if (!Simulation.DemoMode)
        //        Help.Show();
        //}


        //public void Hide()
        //{
        //    Simulation.Scene.Remove(Scenario.Background);

        //    if (!Simulation.DemoMode)
        //        Help.Hide();
        //}


        public void Draw()
        {
            Simulation.Scene.Add(Scenario.Background);

            if (!Simulation.DemoMode && Help.Active)
                Help.Draw();
        }


        public void doWaveEnded()
        {
            WavesCounter++;

            if (WavesCounter == Waves.Count && State == GameState.Running && !Simulation.DemoMode && !EditorMode)
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


        public void doEnemyReachedEnd(Enemy enemy, CelestialBody celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!(celestialBody is AsteroidBelt))
                celestialBody.DoHit(enemy);

            if (!Simulation.DemoMode && Simulation.Etat != GameState.Lost)
            {
                Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteTouche");
            }

            if (celestialBody == CelestialBodyToProtect)
                CommonStash.Lives = (int) CelestialBodyToProtect.LifePoints; //correct de caster?

            if (CommonStash.Lives <= 0 && State == GameState.Running && !Simulation.DemoMode && !EditorMode)
            {
                CelestialBodyToProtect.DoDie();
                Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteExplose");
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

            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody == null)
                return;

            Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteExplose");

            if (celestialBody == CelestialBodyToProtect && !Simulation.DemoMode && !EditorMode)
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


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp.Type == PowerUpType.Shield)
            {
                ((PowerUpShield) powerUp).ToProtect = CelestialBodyToProtect;
                ((PowerUpShield) powerUp).Bullet.Position = CelestialBodyToProtect.Position;
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

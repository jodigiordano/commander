namespace EphemereGames.Commander.Simulation
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


    class LevelsController
    {
        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        public Help Help;

        public List<CelestialBody> CelestialBodies                   { get { return Level.PlanetarySystem; } }
        public InfiniteWave InfiniteWaves                         { get { return Level.InfiniteWaves; } }
        public LinkedList<Wave> Waves                               { get { return Level.Waves; } }
        public CommonStash CommonStash                              { get { return Level.CommonStash; } }
        public List<Turret> StartingTurrets                         { get { return Level.Turrets; } }
        public CelestialBody CelestialBodyToProtect                  { get { return Level.CelestialBodyToProtect; } }
        public Dictionary<TurretType, Turret> AvailableTurrets      { get { return Level.AvailableTurrets; } }
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps   { get { return Level.AvailablePowerUps; } }

        public GameState State;
        public Level Level;

        private Simulator Simulation;
        private int WavesCounter;
        private double ElapsedTime;

        private bool HelpSaved;


        public LevelsController(Simulator simulation)
        {
            Simulation = simulation;
        }


        public void Initialize()
        {
            WavesCounter = 0;
            ElapsedTime = 0;

            if (Main.SaveGame.Tutorials.ContainsKey(Level.Id) && Main.SaveGame.Tutorials[Level.Id] > 2)
            {
                Help = new Help(Simulation, new List<string>());
                HelpSaved = true;
            }

            else
            {
                Help = new Help(Simulation, Level.HelpTexts);
                HelpSaved = false;
            }
        }


        public void Update(GameTime gameTime)
        {
            if (Help.Active)
                Help.Update(gameTime);
            else if (!HelpSaved)
            {
                if (!Main.SaveGame.Tutorials.ContainsKey(Level.Id))
                    Main.SaveGame.Tutorials.Add(new KeyValuePair<int,int>(Level.Id, 0));

                Main.SaveGame.Tutorials[Level.Id]++;

                HelpSaved = true;
            }

            ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }


        public void TriggerNewGameState(GameState state)
        {
            NotifyNewGameState(state);
        }


        public void Draw()
        {
            Simulation.Scene.Add(Level.Background);

            if (!Simulation.DemoMode && Help.Active)
                Help.Draw();
        }


        public void doWaveEnded()
        {
            WavesCounter++;

            if (WavesCounter == Waves.Count && State == GameState.Running && !Simulation.DemoMode && !Simulation.EditorMode)
            {
                State = GameState.Won;

                if (!Main.SaveGame.Progress.ContainsKey(Level.Id))
                    Main.SaveGame.Progress.Add(Level.Id, 0);

                if (Main.SaveGame.Progress[Level.Id] < 0)
                    Main.SaveGame.Progress[Level.Id] = Math.Abs(Main.SaveGame.Progress[Level.Id]);
                else if (Main.SaveGame.Progress[Level.Id] == 0)
                    Main.SaveGame.Progress[Level.Id] = 1;

                ComputeFinalScore();

                NotifyNewGameState(State);
            }
        }


        public void doEnemyReachedEnd(Enemy enemy, CelestialBody celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!(celestialBody is AsteroidBelt))
                celestialBody.DoHit(enemy);

            if (!Simulation.DemoMode && Simulation.State != GameState.Lost)
            {
                Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteTouche");
            }

            if (celestialBody == CelestialBodyToProtect)
                CommonStash.Lives = (int) CelestialBodyToProtect.LifePoints; //correct de caster?

            if (CommonStash.Lives <= 0 && State == GameState.Running && !Simulation.DemoMode && !Simulation.EditorMode)
            {
                CelestialBodyToProtect.DoDie();
                Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteExplose");
                State = GameState.Lost;

                if (!Main.SaveGame.Progress.ContainsKey(Level.Id))
                    Main.SaveGame.Progress.Add(Level.Id, 0);

                if ((Main.SaveGame.Progress[Level.Id] <= 0))
                    Main.SaveGame.Progress[Level.Id] -= 1;

                ComputeFinalScore();

                NotifyNewGameState(State);
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

            if (celestialBody == CelestialBodyToProtect && !Simulation.DemoMode && !Simulation.EditorMode)
            {
                State = GameState.Lost;

                if (!Main.SaveGame.Progress.ContainsKey(Level.Id))
                    Main.SaveGame.Progress.Add(Level.Id, 0);

                if ((Main.SaveGame.Progress[Level.Id] <= 0))
                    Main.SaveGame.Progress[Level.Id] -= 1;

                ComputeFinalScore();

                NotifyNewGameState(State);
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


        private void ComputeFinalScore()
        {
            ElapsedTime = Math.Min(Level.ParTime, ElapsedTime);

            Level.CommonStash.TotalScore += Level.CommonStash.Lives * 50;
            Level.CommonStash.TotalScore += Level.CommonStash.Cash;

            if (State == GameState.Won)
            {
                Level.CommonStash.TimeLeft = (int)((Level.ParTime - ElapsedTime) / 100);
                Level.CommonStash.TotalScore += Level.CommonStash.TimeLeft;
            }

            NotifyCommonStashChanged(Level.CommonStash);
        }


        private void NotifyCommonStashChanged(CommonStash stash)
        {
            if (CommonStashChanged != null)
                CommonStashChanged(stash);
        }


        private void NotifyNewGameState(GameState newState)
        {
            if (NewGameState != null)
                NewGameState(newState);
        }
    }
}

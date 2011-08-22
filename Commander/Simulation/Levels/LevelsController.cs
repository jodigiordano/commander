namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Physics;


    class LevelsController
    {
        public GameState State;
        public Level Level;

        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        public Help Help;

        public List<CelestialBody> CelestialBodies                   { get { return Level.PlanetarySystem; } }
        public InfiniteWave InfiniteWaves                            { get { return Level.InfiniteWaves; } }
        public LinkedList<Wave> Waves                                { get { return Level.Waves; } }
        public CommonStash CommonStash                               { get { return Level.CommonStash; } }
        public List<Turret> StartingTurrets                          { get { return Level.Turrets; } }
        public CelestialBody CelestialBodyToProtect                  { get { return Level.CelestialBodyToProtect; } }
        public Dictionary<TurretType, Turret> AvailableTurrets       { get { return Level.AvailableTurrets; } }
        public Dictionary<PowerUpType, PowerUp> AvailablePowerUps    { get { return Level.AvailablePowerUps; } }

        private Simulator Simulator;
        private int WavesCounter;
        private double ElapsedTime;

        private bool HelpSaved;


        public LevelsController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {
            WavesCounter = 0;
            ElapsedTime = 0;

            if (Simulator.DemoMode ||
                Simulator.WorldMode ||
                Simulator.EditorMode ||
                Main.SaveGameController.ShowTutorial(Level.Id))
            {
                Help = new Help(Simulator, new List<string>());
                HelpSaved = true;
            }

            else
            {
                Help = new Help(Simulator, Level.HelpTexts);
                HelpSaved = false;
            }
        }


        public void Update()
        {
            if (Help.Active)
                Help.Update();
            else if (!HelpSaved)
            {
                Main.SaveGameController.SyncTutorial(Level.Id);

                HelpSaved = true;
            }

            ElapsedTime += Preferences.TargetElapsedTimeMs;
        }


        public void TriggerNewGameState(GameState state)
        {
            NotifyNewGameState(state);
        }


        public void Draw()
        {
            Simulator.Scene.BeginBackground();
            Simulator.Scene.Add(Level.Background);
            Simulator.Scene.EndBackground();

            if (!Simulator.DemoMode && Help.Active)
                Help.Draw();
        }


        public void DoWaveEnded()
        {
            WavesCounter++;

            if (WavesCounter == Waves.Count && State == GameState.Running && !Simulator.DemoMode && !Simulator.EditorMode)
            {
                State = CelestialBodyToProtect.Alive ? GameState.Won : GameState.Lost;

                ComputeFinalScore();

                NotifyNewGameState(State);
            }
        }


        public void DoEnemyReachedEnd(Enemy enemy, CelestialBody celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (celestialBody == null)
                return;

            if (!(celestialBody is AsteroidBelt))
                celestialBody.DoHit(enemy);

            if (!Simulator.DemoMode && Simulator.State != GameState.Lost)
            {
                Audio.PlaySfx(@"sfxCorpsCelesteTouche");
            }

            if (Simulator.EditorMode && celestialBody == CelestialBodyToProtect)
                celestialBody.LifePoints = CommonStash.Lives;

            if (Simulator.EditorMode || Simulator.DemoMode)
                return;

            if (celestialBody == CelestialBodyToProtect)
                CommonStash.Lives = (int) CelestialBodyToProtect.LifePoints;

            if (CommonStash.Lives <= 0 && State == GameState.Running)
            {
                CelestialBodyToProtect.DoDie();
                Audio.PlaySfx(@"sfxCorpsCelesteExplose");
                State = GameState.Lost;

                ComputeFinalScore();

                NotifyNewGameState(State);
            }
        }
        

        public void DoObjectDestroyed(ICollidable obj)
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;

            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody == null)
                return;

            if (!Simulator.CutsceneMode)
                Audio.PlaySfx(@"sfxCorpsCelesteExplose");

            if (celestialBody == CelestialBodyToProtect && !Simulator.DemoMode && !Simulator.EditorMode)
            {
                State = GameState.Lost;

                ComputeFinalScore();

                NotifyNewGameState(State);
            }
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.Shield)
            {
                ((PowerUpShield) powerUp).ToProtect = CelestialBodyToProtect;
                ((PowerUpShield) powerUp).Bullet.Position = CelestialBodyToProtect.Position;
            }
        }


        public void DoEditorCommandExecuted(EditorCommand c)
        {
            switch (c.Type)
            {
                case EditorCommandType.CelestialBody:
                    DoEditorCelestialBodyCommand((EditorCelestialBodyCommand) c);
                    break;

                case EditorCommandType.Player:
                    DoEditorPlayerCommand((EditorPlayerCommand) c);
                    break;
            }
        }


        private void DoEditorCelestialBodyCommand(EditorCelestialBodyCommand command)
        {
            if (command.Name == "Clear")
            {
                Level.CelestialBodyToProtect = null;
            }

            else if (command.Name == "AddPlanet")
            {
                if (Level.CelestialBodyToProtect == null)
                {
                    Level.CelestialBodyToProtect = command.CelestialBody;
                    Level.CelestialBodyToProtect.LifePoints = CommonStash.Lives;
                }
            }

            else if (command.Name == "PushFirst")
            {
                Level.CelestialBodyToProtect = PlanetarySystemController.GetCelestialBodyWithHighestPathPriority(CelestialBodies);

                if (Level.CelestialBodyToProtect == null)
                    Level.CelestialBodyToProtect = PlanetarySystemController.GetAsteroidBelt(CelestialBodies);
                
                Level.CelestialBodyToProtect.LifePoints = CommonStash.Lives;
            }


            else if (command.Name == "PushLast")
            {
                Level.CelestialBodyToProtect = command.CelestialBody;
                Level.CelestialBodyToProtect.LifePoints = CommonStash.Lives;
            }


            else if (command.Name == "Remove")
            {
                Level.CelestialBodyToProtect = PlanetarySystemController.GetAliveCelestialBodyWithHighestPathPriority(CelestialBodies);

                if (Level.CelestialBodyToProtect == null)
                    Level.CelestialBodyToProtect = PlanetarySystemController.GetAsteroidBelt(CelestialBodies);

                if (Level.CelestialBodyToProtect != null)
                    Level.CelestialBodyToProtect.LifePoints = CommonStash.Lives;
            }
        }


        private void DoEditorPlayerCommand(EditorPlayerCommand command)
        {
            if (command.Name == "AddOrRemoveLives")
            {
                CommonStash.Lives = command.LifePoints;

                if (CelestialBodyToProtect != null)
                    CelestialBodyToProtect.LifePoints = command.LifePoints;
            }

            else if (command.Name == "AddOrRemoveCash")
            {
                CommonStash.Cash = command.Cash;
            }

            else if (command.Name == "AddOrRemoveMinerals")
            {
                Level.Minerals = command.Minerals;
            }

            else if (command.Name == "AddOrRemoveLifePacks")
            {
                Level.LifePacks = command.LifePacks;
            }

            else if (command.Name == "AddOrRemoveBulletDamage")
            {
                Level.SaveBulletDamage = true;
                Level.BulletDamage = command.BulletDamage;
            }
        }


        private void ComputeFinalScore()
        {
            ElapsedTime = Math.Min(Level.ParTime, ElapsedTime * 0.75);


            // get data
            var time = (State == GameState.Won) ? Level.ParTime - ElapsedTime : 0;
            var cash = Level.CommonStash.Cash;
            var lives = Level.CommonStash.Lives;

            // set totals
            Level.CommonStash.TotalCash = Level.GetTotalCash(cash);
            Level.CommonStash.TotalLives = Level.GetTotalLives(lives);
            Level.CommonStash.TotalTime = Level.GetTotalTime(time);
            Level.CommonStash.TotalScore = Level.GetTotalScore(time, cash, lives);
            Level.CommonStash.PotentialScore = Level.GetPotentialScore();

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

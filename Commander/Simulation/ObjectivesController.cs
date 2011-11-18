namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;


    class ObjectivesController
    {
        public GameState State;

        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        private Simulator Simulator;
        private int WavesCounter;
        private double ElapsedTime;

        private Level Level { get { return Simulator.Data.Level; } }


        public ObjectivesController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {
            WavesCounter = 0;
            ElapsedTime = 0;

            if (Simulator.GameMode && !Simulator.EditingMode && Level.HelpText != "")
            {
                Simulator.Scene.Add(new HelpAnimation(Level.HelpText, VisualPriorities.Foreground.HelpMessage));
            }
        }


        public void Update()
        {
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
        }


        public void DoWaveEnded()
        {
            WavesCounter++;

            if (IsGameOver)
                DoGameEnded();
        }


        private bool IsGameOver
        {
            get { return Simulator.GameMode && !Simulator.EditingMode && WavesCounter == Level.Waves.Count && State == GameState.Running; }
        }


        private void DoGameEnded()
        {
            State = Level.CelestialBodyToProtect != null && Level.CelestialBodyToProtect.Alive ? GameState.Won : GameState.Lost;

            ComputeFinalScore();

            NotifyNewGameState(State);
        }


        public void DoObjectHit(ICollidable obj)
        {
            var celestialBody = obj as CelestialBody;

            if (celestialBody == null)
                return;

            if (Simulator.EditingMode && celestialBody == Level.CelestialBodyToProtect)
                celestialBody.LifePoints = Level.CommonStash.Lives;

            if (Simulator.EditingMode || !Simulator.GameMode)
                return;

            if (celestialBody == Level.CelestialBodyToProtect)
                Level.CommonStash.Lives = (int) Level.CelestialBodyToProtect.LifePoints;
        }
        

        public void DoObjectDestroyed(ICollidable obj)
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;

            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody == null || !Simulator.GameMode ||
                Simulator.EditingMode || celestialBody != Level.CelestialBodyToProtect)
                return;

            State = GameState.Lost;
            Level.CommonStash.Lives = (int) Level.CelestialBodyToProtect.LifePoints;

            ComputeFinalScore();

            NotifyNewGameState(State);
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.Shield)
            {
                ((PowerUpShield) powerUp).ToProtect = Level.CelestialBodyToProtect;
                ((PowerUpShield) powerUp).Bullet.Position = Level.CelestialBodyToProtect.Position;
            }
        }


        public void DoEditorCommandExecuted(EditorCommand c)
        {
            if (c is EditorCelestialBodyAddCommand)
            {
                var command = (EditorCelestialBodyAddCommand) c;

                if (Level.CelestialBodyToProtect == null)
                {
                    Level.CelestialBodyToProtect = command.CelestialBody;
                    Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;
                }

                return;
            }

            if (c is EditorCelestialBodyPushFirstCommand)
            {
                Level.CelestialBodyToProtect = PlanetarySystemController.GetCelestialBodyWithHighestPathPriority(Level.PlanetarySystem);

                if (Level.CelestialBodyToProtect == null)
                    Level.CelestialBodyToProtect = PlanetarySystemController.GetAsteroidBelt(Level.PlanetarySystem);

                Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;

                return;
            }


            if (c is EditorCelestialBodyPushLastCommand)
            {
                var command = (EditorCelestialBodyPushLastCommand) c;
                
                Level.CelestialBodyToProtect = command.CelestialBody;
                Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;

                return;
            }


            if (c is EditorCelestialBodyRemoveCommand)
            {
                Level.CelestialBodyToProtect = PlanetarySystemController.GetAliveCelestialBodyWithHighestPathPriority(Level.PlanetarySystem);

                if (Level.CelestialBodyToProtect == null)
                    Level.CelestialBodyToProtect = PlanetarySystemController.GetAsteroidBelt(Level.PlanetarySystem);

                if (Level.CelestialBodyToProtect != null)
                    Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;

                return;
            }


            if (c is EditorPlayerLifePointsCommand)
            {
                var command = (EditorPlayerLifePointsCommand) c;

                Level.CommonStash.Lives = command.LifePoints;

                if (Level.CelestialBodyToProtect != null)
                    Level.CelestialBodyToProtect.LifePoints = command.LifePoints;

                return;
            }


            if (c is EditorPlayerCashCommand)
            {
                var command = (EditorPlayerCashCommand) c;

                Level.CommonStash.Cash = command.Cash;

                return;
            }

            
            if (c is EditorPlayerMineralsCommand)
            {
                var command = (EditorPlayerMineralsCommand) c;

                Level.Minerals = command.Minerals;

                return;
            }

            
            if (c is EditorPlayerLifePacksCommand)
            {
                var command = (EditorPlayerLifePacksCommand) c;

                Level.LifePacks = command.LifePacks;

                return;
            }

            
            if (c is EditorPlayerBulletDamageCommand)
            {
                var command = (EditorPlayerBulletDamageCommand) c;

                Level.Descriptor.Player.BulletDamage = command.BulletDamage;

                return;
            }
        }


        public void ComputeFinalScore()
        {
            var parTime = Level.Descriptor.GetParTime();

            ElapsedTime = Math.Min(parTime, ElapsedTime * 0.75);


            // get data
            var time = (State == GameState.Won) ? parTime - ElapsedTime : 0;
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

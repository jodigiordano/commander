namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;


    class LevelsController
    {
        public GameState State;

        public event NewGameStateHandler NewGameState;
        public event CommonStashHandler CommonStashChanged;

        private Simulator Simulator;
        private int WavesCounter;
        private double ElapsedTime;

        private Level Level { get { return Simulator.Data.Level; } }


        public LevelsController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {
            WavesCounter = 0;
            ElapsedTime = 0;

            if (!Simulator.DemoMode && !Simulator.WorldMode && !Simulator.EditorMode && Level.HelpText != "")
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

            if (WavesCounter == Level.Waves.Count && State == GameState.Running && !Simulator.DemoMode && !Simulator.EditorMode)
            {
                State = Level.CelestialBodyToProtect.Alive ? GameState.Won : GameState.Lost;

                ComputeFinalScore();

                NotifyNewGameState(State);
            }
        }


        public void DoObjectHit(ICollidable obj)
        {
            var celestialBody = obj as CelestialBody;

            if (celestialBody == null)
                return;

            if (Simulator.EditorMode && celestialBody == Level.CelestialBodyToProtect)
                celestialBody.LifePoints = Level.CommonStash.Lives;

            if (Simulator.EditorMode || Simulator.DemoMode)
                return;

            if (celestialBody == Level.CelestialBodyToProtect)
                Level.CommonStash.Lives = (int) Level.CelestialBodyToProtect.LifePoints;
        }
        

        public void DoObjectDestroyed(ICollidable obj)
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;

            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody == null || Simulator.DemoMode ||
                Simulator.EditorMode || celestialBody != Level.CelestialBodyToProtect)
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
            if (c is EditorCelestialBodyCommand)
                DoEditorCelestialBodyCommand((EditorCelestialBodyCommand) c);
            else if (c is EditorPlayerCommand)
                DoEditorPlayerCommand((EditorPlayerCommand) c);
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
                    Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;
                }
            }

            else if (command.Name == "PushFirst")
            {
                Level.CelestialBodyToProtect = PlanetarySystemController.GetCelestialBodyWithHighestPathPriority(Level.PlanetarySystem);

                if (Level.CelestialBodyToProtect == null)
                    Level.CelestialBodyToProtect = PlanetarySystemController.GetAsteroidBelt(Level.PlanetarySystem);

                Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;
            }


            else if (command.Name == "PushLast")
            {
                Level.CelestialBodyToProtect = command.CelestialBody;
                Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;
            }


            else if (command.Name == "Remove")
            {
                Level.CelestialBodyToProtect = PlanetarySystemController.GetAliveCelestialBodyWithHighestPathPriority(Level.PlanetarySystem);

                if (Level.CelestialBodyToProtect == null)
                    Level.CelestialBodyToProtect = PlanetarySystemController.GetAsteroidBelt(Level.PlanetarySystem);

                if (Level.CelestialBodyToProtect != null)
                    Level.CelestialBodyToProtect.LifePoints = Level.CommonStash.Lives;
            }
        }


        private void DoEditorPlayerCommand(EditorPlayerCommand command)
        {
            if (command.Name == "AddOrRemoveLives")
            {
                Level.CommonStash.Lives = command.LifePoints;

                if (Level.CelestialBodyToProtect != null)
                    Level.CelestialBodyToProtect.LifePoints = command.LifePoints;
            }

            else if (command.Name == "AddOrRemoveCash")
            {
                Level.CommonStash.Cash = command.Cash;
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
                Level.BulletDamage = command.BulletDamage;
            }
        }


        public void ComputeFinalScore()
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

namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


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

            if (Main.SaveGame.Tutorials.ContainsKey(Level.Id) && Main.SaveGame.Tutorials[Level.Id] > 2)
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
            Simulator.Scene.Add(Level.Background);

            if (!Simulator.DemoMode && Help.Active)
                Help.Draw();
        }


        public void DoWaveEnded()
        {
            WavesCounter++;

            if (WavesCounter == Waves.Count && State == GameState.Running && !Simulator.DemoMode && !Simulator.EditorMode)
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


        public void DoEnemyReachedEnd(Enemy enemy, CelestialBody celestialBody)
        {
            if (State == GameState.Won)
                return;

            if (!(celestialBody is AsteroidBelt))
                celestialBody.DoHit(enemy);

            if (!Simulator.DemoMode && Simulator.State != GameState.Lost)
            {
                Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteTouche");
            }

            if (celestialBody == CelestialBodyToProtect)
                CommonStash.Lives = (int) CelestialBodyToProtect.LifePoints; //correct de caster?

            if (CommonStash.Lives <= 0 && State == GameState.Running && !Simulator.DemoMode && !Simulator.EditorMode)
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
        

        public void DoObjectDestroyed(IObjetPhysique obj)
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;

            CelestialBody celestialBody = obj as CelestialBody;

            if (celestialBody == null)
                return;

            Audio.PlaySfx(@"Partie", @"sfxCorpsCelesteExplose");

            if (celestialBody == CelestialBodyToProtect && !Simulator.DemoMode && !Simulator.EditorMode)
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

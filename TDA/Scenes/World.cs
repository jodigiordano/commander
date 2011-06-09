namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class World
    {
        public int Id { get { return Descriptor.Id; } }
        public Simulation Simulation;
        public Dictionary<CorpsCeleste, int> WarpsCelestialBodies;

        private Main Main;
        private Scene Scene;
        private WorldDescriptor Descriptor;
        private Dictionary<string, ScenarioDescriptor> LevelsDescriptors;
        private List<KeyAndValue<CorpsCeleste, Image>> LevelsStates;
        private Dictionary<string, int> Warps;


        public World(Main main, Scene scene, WorldDescriptor descriptor)
        {
            Main = main;
            Scene = scene;
            Descriptor = descriptor;

            Simulation = new Simulation(main, scene, descriptor.SimulationDescription);
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.DemoMode = true;
            Simulation.WorldMode = true;

            LevelsDescriptors = new Dictionary<string, ScenarioDescriptor>();
            Warps = new Dictionary<string, int>();

            foreach (var level in descriptor.Levels)
            {
                ScenarioDescriptor d = ScenariosFactory.GetLevelScenario(level);
                LevelsDescriptors.Add(d.Mission, d);
            }

            foreach (var level in descriptor.Warps)
            {
                ScenarioDescriptor d = ScenariosFactory.GetLevelScenario(level.Key);
                LevelsDescriptors.Add(d.Mission, d);
                Warps.Add(d.Mission, level.Value);
            }


            LevelsStates = new List<KeyAndValue<CorpsCeleste, Image>>();
            WarpsCelestialBodies = new Dictionary<CorpsCeleste, int>();

            foreach (var celestialBody in Simulation.PlanetarySystemController.CelestialBodies)
            {
                if (LevelsDescriptors.ContainsKey(celestialBody.Nom) && !(celestialBody is TrouRose))
                    LevelsStates.Add(new KeyAndValue<CorpsCeleste, Image>(celestialBody, null));

                if (celestialBody is TrouRose)
                    WarpsCelestialBodies.Add(celestialBody, Warps[celestialBody.Nom]);
            }


            InitLevelsStates();
        }


        public bool Unlocked
        {
            get
            {
                int save = 0;
                bool unlocked = true;

                foreach (var level in Descriptor.UnlockedCondition)
                    if (!Main.SaveGame.Progress.TryGetValue(level, out save) || save <= 0)
                    {
                        unlocked = false;
                        break;
                    }
              
                return unlocked;
            }
        }


        public ScenarioDescriptor LevelSelected
        {
            get
            {
                return Simulation.SelectedCelestialBody != null ? LevelsDescriptors[Simulation.SelectedCelestialBody.Nom] : null;
            }
        }


        public int WorldSelected
        {
            get
            {
                return (Simulation.SelectedCelestialBody != null && Simulation.SelectedCelestialBody is TrouRose) ? Warps[Simulation.SelectedCelestialBody.Nom] : -1;
            }
        }


        public bool PausedGameSelected
        {
            get
            {
                return Main.GameInProgress != null && LevelSelected != null && Main.GameInProgress.Simulation.DescriptionScenario.Id == LevelSelected.Id;
            }
        }


        public void ShowWarpBlockedMessage()
        {
            Simulation.MessagesController.ShowMessage(Simulation.SelectedCelestialBody, Descriptor.WarpBlockedMessage, 5000, -1);
        }


        public void Update(GameTime gameTime)
        {
            if (LevelSelected != null)
            {
                Simulation.DemoModeSelectedScenario.Id = LevelSelected.Id;
                Simulation.DemoModeSelectedScenario.Mission = LevelSelected.Mission;
                Simulation.DemoModeSelectedScenario.Difficulty = LevelSelected.Difficulty;
                Simulation.DemoModeSelectedScenario.Waves = LevelSelected.Waves;
                Simulation.DemoModeSelectedScenario.Player = LevelSelected.Player;
                Simulation.DemoModeSelectedScenario.LifePacks = LevelSelected.LifePacks;
            }

            if (Main.GameInProgress != null)
            {
                for (int i = 0; i < Simulation.PlanetarySystemController.CelestialBodies.Count; i++)
                {
                    CorpsCeleste c = Simulation.PlanetarySystemController.CelestialBodies[i];

                    if (c.Nom == Main.GameInProgress.Simulation.DescriptionScenario.Mission)
                    {
                        Simulation.CelestialBodyPausedGame = c;
                        break;
                    }
                }

                if (Main.GameInProgress.State != GameState.Paused || PausedGameSelected)
                    Simulation.MessagesController.StopPausedMessage();
                else
                    Simulation.MessagesController.DisplayPausedMessage();
            }

            else
            {
                Simulation.CelestialBodyPausedGame = null;
            }

            Simulation.Update(gameTime);
        }


        public void Draw()
        {
            Simulation.Draw();

            foreach (var kvp in LevelsStates)
                Scene.Add(kvp.Value);
        }


        public void InitLevelsStates()
        {
            foreach (var kvp in LevelsStates)
            {
                var descriptor = LevelsDescriptors[kvp.Key.Nom];

                int value = 0;
                bool done = Main.SaveGame.Progress.TryGetValue(descriptor.Id, out value) && value > 0;

                kvp.Value = new Image((done) ? "LevelDone" : "LevelNotDone",
                    kvp.Key.Position + new Vector3(kvp.Key.Circle.Radius, kvp.Key.Circle.Radius, 0) +
                    ((kvp.Key.Circle.Radius < (int) Size.Normal) ? new Vector3(-10, -10, 0) : new Vector3(-30, -30, 0)));

                kvp.Value.VisualPriority = kvp.Key.Representation.VisualPriority - 0.0001f;
                kvp.Value.SizeX = (kvp.Key.Circle.Radius < (int) Size.Normal) ? 0.5f : 0.80f;
            }
        }
    }
}

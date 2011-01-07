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
        private Dictionary<string, DescripteurScenario> LevelsDescriptors;
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
            Simulation.ModeDemo = true;
            Simulation.WorldMode = true;

            LevelsDescriptors = new Dictionary<string, DescripteurScenario>();
            Warps = new Dictionary<string, int>();

            foreach (var level in descriptor.Levels)
            {
                DescripteurScenario d = FactoryScenarios.GetLevelScenario(level);
                LevelsDescriptors.Add(d.Mission, d);
            }

            foreach (var level in descriptor.Warps)
            {
                DescripteurScenario d = FactoryScenarios.GetLevelScenario(level.Key);
                LevelsDescriptors.Add(d.Mission, d);
                Warps.Add(d.Mission, level.Value);
            }


            LevelsStates = new List<KeyAndValue<CorpsCeleste, Image>>();
            WarpsCelestialBodies = new Dictionary<CorpsCeleste, int>();

            foreach (var celestialBody in Simulation.ControleurSystemePlanetaire.CelestialBodies)
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


        public DescripteurScenario LevelSelected
        {
            get
            {
                return Simulation.CorpsCelesteSelectionne != null ? LevelsDescriptors[Simulation.CorpsCelesteSelectionne.Nom] : null;
            }
        }


        public int WorldSelected
        {
            get
            {
                return (Simulation.CorpsCelesteSelectionne != null && Simulation.CorpsCelesteSelectionne is TrouRose) ? Warps[Simulation.CorpsCelesteSelectionne.Nom] : -1;
            }
        }


        public bool PausedGameSelected
        {
            get
            {
                return Main.GameInProgress != null && LevelSelected != null && Main.GameInProgress.Simulation.DescriptionScenario.Numero == LevelSelected.Numero;
            }
        }


        public void ShowWarpBlockedMessage()
        {
            Simulation.ControleurMessages.afficherMessage(Simulation.CorpsCelesteSelectionne, Descriptor.WarpBlockedMessage, 5000, -1);
        }


        public void Update(GameTime gameTime)
        {
            if (LevelSelected != null)
            {
                Simulation.DemoModeSelectedScenario.Numero = LevelSelected.Numero;
                Simulation.DemoModeSelectedScenario.Mission = LevelSelected.Mission;
                Simulation.DemoModeSelectedScenario.Difficulte = LevelSelected.Difficulte;
                Simulation.DemoModeSelectedScenario.Waves = LevelSelected.Waves;
                Simulation.DemoModeSelectedScenario.Joueur = LevelSelected.Joueur;
                Simulation.DemoModeSelectedScenario.NbPackViesDonnes = LevelSelected.NbPackViesDonnes;
            }

            if (Main.GameInProgress != null)
            {
                Simulation.CelestialBodyPausedGame = Simulation.ControleurSystemePlanetaire.CelestialBodies.Find(e => e.Nom == Main.GameInProgress.Simulation.DescriptionScenario.Mission);

                if (Main.GameInProgress.State != GameState.Paused || PausedGameSelected)
                    Simulation.ControleurMessages.StopPausedMessage();
                else
                    Simulation.ControleurMessages.DisplayPausedMessage();
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
                Scene.ajouterScenable(kvp.Value);
        }


        public void InitLevelsStates()
        {
            foreach (var kvp in LevelsStates)
            {
                var descriptor = LevelsDescriptors[kvp.Key.Nom];

                int value = 0;
                bool done = Main.SaveGame.Progress.TryGetValue(descriptor.Numero, out value) && value > 0;

                kvp.Value = new Image((done) ? "LevelDone" : "LevelNotDone",
                    kvp.Key.Position + new Vector3(kvp.Key.Cercle.Radius, kvp.Key.Cercle.Radius, 0) +
                    ((kvp.Key.Cercle.Radius < (int) Taille.Moyenne) ? new Vector3(-10, -10, 0) : new Vector3(-30, -30, 0)));

                kvp.Value.VisualPriority = kvp.Key.Representation.VisualPriority - 0.0001f;
                kvp.Value.SizeX = (kvp.Key.Cercle.Radius < (int) Taille.Moyenne) ? 0.5f : 0.80f;
            }
        }
    }
}

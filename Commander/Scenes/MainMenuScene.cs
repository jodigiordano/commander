namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    

    class MainMenuScene : Scene
    {
        private Simulator Simulator;

        private Text Title;        
        private Image Filter;


        private enum State
        {
            ConnectPlayer,
            ConnectingPlayer,
            LoadSaveGame,
            PlayerConnected
        }

        private State SceneState;
        private Translator PressStart;
        private Dictionary<Text, CelestialBody> Choices;


        public MainMenuScene()
            : base(1280, 720)
        {
            Name = "Menu";

            Title = new Text("Commander", "PixelBig")
            {
                SizeX = 3,
                VisualPriority = Preferences.PrioriteGUIMenuPrincipal,
            }.CenterIt();

            Filter = new Image("PixelBlanc")
            {
                Size = new Vector2(1800, 200),
                Color = new Color(0, 0, 0, 200),
                VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.00001,
                Origin = Vector2.Zero
            };

            LevelDescriptor levelDescriptor = Main.LevelsFactory.Menu;

#if !DEBUG
            levelDescriptor.PlanetarySystem[5].CanSelect = false;
#endif

            Simulator = new Simulator(this, levelDescriptor)
            {
                DemoMode = true,
                EnableInputs = false
            };
            Simulator.Initialize();
            Inputs.AddListener(Simulator);

            SceneState = State.ConnectPlayer;
            InitPressStart();

            Choices = new Dictionary<Text, CelestialBody>();

            foreach (var c in Simulator.PlanetarySystemController.CelestialBodies)
            {
                if (c is AsteroidBelt)
                    continue;

                var text = new Text(c.Name, "Pixelite")
                {
                    SizeX = 3,
                    VisualPriority = Preferences.PrioriteFondEcran - 0.01f
                }.CenterIt();

                Choices.Add(text, c);
            }
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();

            switch (SceneState)
            {
                case State.ConnectPlayer:                    
                    VisualEffects.Add(PressStart.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));
                    VisualEffects.Add(PressStart.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeInFrom0(255, 500, 1000));
                    VisualEffects.Add(Title, Core.Visual.VisualEffects.Fade(Title.Alpha, 255, 0, 1000));
                    VisualEffects.Add(Filter, Core.Visual.VisualEffects.Fade(Filter.Alpha, 100, 0, 500));

                    foreach (var kvp in Choices)
                        VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.Fade(kvp.Key.Alpha, 0, 0, 500));

                    MovePathEffect mpe = new MovePathEffect()
                    {
                        StartAt = 0,
                        PointAt = false,
                        Delay = 0,
                        Length = 10000,
                        Progress = Core.Utilities.Effect<IPhysicalObject>.ProgressType.Linear,
                        InnerPath = new Path2D(new List<Vector2>()
                        {
                            new Vector2(-1920, -100),
                            new Vector2(-1000, -100),
                            new Vector2(-740, -100)
                        }, new List<double>()
                        {
                            0,
                            600,
                            1200
                        })
                    };

                    PhysicalEffects.Add(Filter, mpe);

                    SceneState = State.ConnectingPlayer;
                    break;

                case State.ConnectingPlayer:
                    PressStart.Update();
                    break;


                case State.LoadSaveGame:
                    if (Persistence.IsDataLoaded("Save"))
                    {
                        VisualEffects.Add(PressStart.PartieTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(PressStart.PartieTraduite.Alpha, 0, 1000));
                        VisualEffects.Add(PressStart.PartieNonTraduite, EphemereGames.Core.Visual.VisualEffects.FadeOutTo0(PressStart.PartieTraduite.Alpha, 0, 1000));
                        VisualEffects.Add(Title, Core.Visual.VisualEffects.FadeOutTo0(Title.Alpha, 0, 1000));
                        VisualEffects.Add(Filter, Core.Visual.VisualEffects.FadeOutTo0(Filter.Alpha, 0, 500));

                        foreach (var kvp in Choices)
                            VisualEffects.Add(kvp.Key, Core.Visual.VisualEffects.Fade(kvp.Key.Alpha, 100, Main.Random.Next(0, 500), 500));

                        if (!Simulator.EnableInputs)
                            Simulator.SyncPlayers();

                        Simulator.EnableInputs = true;
                        SceneState = State.PlayerConnected;

                        Simulator.ShowHelpBarMessage(HelpBarMessage.MoveYourSpaceship);
                        Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 255, 1000);
                    }
                    break;
            }
        }


        protected override void UpdateVisual()
        {
            foreach (var kvp in Choices)
            {
                kvp.Key.Position = kvp.Value.Position + new Vector3(0, kvp.Value.Circle.Radius + 10, 0);
                Add(kvp.Key);
            }


            foreach (var player in Inputs.ConnectedPlayers)
            {
                CelestialBody c = Simulator.GetSelectedCelestialBody(player);

                if (c != null)
                {
                    foreach (var kvp in Choices)
                        if (kvp.Key.Data == c.Name)
                        {
                            Add(kvp.Key);
                            break;
                        }
                }
            }

            Add(Title);
            Add(Filter);
            Simulator.Draw();
            PressStart.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.SyncPlayers();

            Main.MusicController.PlayMusic(false);

            if (Inputs.ConnectedPlayers.Count == 0)
                InitConnectFirstPlayer();
            else
            {
                Simulator.EnableInputs = true;
            }
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Main.MusicController.PauseMusic();

            Simulator.EnableInputs = false;
        }


        private void InitPressStart()
        {
            PressStart = new Translator
            (this, new Vector3(0, 50, 0), "Alien", new Color(234, 196, 28, 0), "Pixelite", new Color(255, 255, 255, 0), (Preferences.Target == Core.Utilities.Setting.Xbox360) ? "Press a button to start your engine" : "Click a button to start your engine", 3, true, 3000, 250, Preferences.PrioriteGUIMenuPrincipal);
            PressStart.Centre = true;
        }


        #region Input Handling

        public override void PlayerMouseConnectionRequested(Core.Input.Player p, MouseButton button)
        {
            if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void PlayerGamePadConnectionRequested(Core.Input.Player p, Buttons button)
        {
            if (p.State == PlayerState.Disconnected)
                p.Connect();
        }


        public override void DoPlayerConnected(Core.Input.Player p)
        {
            if (Inputs.ConnectedPlayers.Count == 1)
                ReloadPlayerData((Player) p);
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Select)
                BeginTransition((Player) p);
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Select)
                BeginTransition((Player) p);

            if (button == GamePadConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            if (key == KeyboardConfiguration.ChangeMusic)
                Main.MusicController.ChangeMusic(false);
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                InitConnectFirstPlayer();
            else if (Main.PlayerSaveGame.Player == player)
            {
                ReloadPlayerData((Player) Inputs.MasterPlayer);
                SceneState = State.LoadSaveGame;
            }
        }


        private void ReloadPlayerData(Player p)
        {
            Main.PlayerSaveGame = new SaveGame(p);
            Persistence.SetPlayerData(Main.PlayerSaveGame);
            Persistence.LoadData("Save");
            SceneState = State.LoadSaveGame;
        }


        private void InitConnectFirstPlayer()
        {
            Simulator.HelpBar.Fade(Simulator.HelpBar.Alpha, 0, 1000);
            InitPressStart();
            SceneState = State.ConnectPlayer;
            Simulator.EnableInputs = false;
            Simulator.SyncPlayers();
        }


        private void BeginTransition(Player player)
        {
            CelestialBody c = Simulator.GetSelectedCelestialBody(player);

            if (c == null)
                return;

#if !DEBUG
            if (c.Name == "editor")
                return;
#endif

            switch (c.Name)
            {
                case "save the world": TransiteTo("Cutscene1"); break;
                case "help": TransiteTo("Aide"); break;
                case "options": TransiteTo("Options"); break;
                case "editor": TransiteTo("Editeur"); break;

                case "quit":
                    if (Preferences.Target == Core.Utilities.Setting.Xbox360 && Main.TrialMode.Active)
                        TransiteTo("Acheter");
                    else
                        Main.Instance.Exit();
                    break;
            }
        }

        #endregion
    }
}

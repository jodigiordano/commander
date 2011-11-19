namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    
    class Data
    {
        public Battlefield Battlefield;
        public Level Level;
        public List<Bullet> Bullets;
        public List<ShootingStar> ShootingStars;
        public Path Path;
        public Path PathPreview;
        public List<Enemy> Enemies;
        public Dictionary<Commander.Player, SimPlayer> Players;
        public Dictionary<string, Panel> Panels;
        public List<Wave> ActiveWaves;
        public int RemainingWaves;


        private Simulator Simulator;


        public Data(Simulator simulator)
        {
            Simulator = simulator;

            Bullets = new List<Bullet>();
            ShootingStars = new List<ShootingStar>();

            Path = new Path(Simulator, new ColorInterpolator(Color.White, Color.Red), 100, BlendType.Add);
            PathPreview = new Path(Simulator, new ColorInterpolator(Color.White, Color.Green), 0, BlendType.Add) { TakeIntoAccountFakeGravTurret = true, TakeIntoAccountFakeGravTurretLv2 = true };

            Enemies = new List<Enemy>();

            Players = new Dictionary<Commander.Player, SimPlayer>();

            Battlefield = new Battlefield(Simulator);

            Panels = new Dictionary<string, Panel>();

            ActiveWaves = new List<Wave>();

            RemainingWaves = -1;
        }


        public void Initialize()
        {
            Bullets.Clear();
            ShootingStars.Clear();

            Battlefield.Initialize(Level);

            Level.Initialize();
            Path.Initialize();
            PathPreview.Initialize();

            Enemies.Clear();

            Players.Clear();

            InitializePanels();

            ActiveWaves.Clear();

            RemainingWaves = Level.InfiniteWaves == null ? Level.Waves.Count : -1;
        }


        public bool HasPlayer(Commander.Player player)
        {
            return Players.ContainsKey(player);
        }


        public SimPlayer GetPlayer(Commander.Player player)
        {
            SimPlayer simPlayer = null;

            return Players.TryGetValue(player, out simPlayer) ? simPlayer : null;
        }


        private void InitializePanels()
        {
            Panels.Clear();

            Panels.Add("Credits", new CreditsPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 550), VisualPriorities.Default.Panel, Color.White));
            Panels.Add("GeneralNews", new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.Panel, Color.White, NewsType.General, "What's up at Ephemere Games"));
            Panels.Add("UpdatesNews", new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.Panel, Color.White, NewsType.Updates, "You've just been updated!"));
            Panels.Add("Options", new OptionsPanel(Simulator, Vector3.Zero, new Vector2(400, 400), VisualPriorities.Default.Panel, Color.White));
            Panels.Add("Pause", new PausePanel(Simulator, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.Panel, Color.White));
            Panels.Add("Controls", new ControlsPanel(Simulator.Scene, Vector3.Zero, new Vector2(600, 700), VisualPriorities.Default.Panel, Color.White));
            Panels.Add("Help", new HelpPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 600), VisualPriorities.Default.Panel, Color.White));
            Panels.Add("Highscores", new HighscoresPanel(Simulator));

            Panels.Add("Login", new LoginPanel(Simulator));
            Panels.Add("Register", new RegisterPanel(Simulator));
            Panels.Add("JumpToWorld", new JumpToWorldPanel(Simulator));
            Panels.Add("VirtualKeyboard", new VirtualKeyboardPanel(Simulator));

            Panels.Add("EditorPlayer", new PlayerPanel(Simulator));
            Panels.Add("EditorTurrets", new TurretsAssetsPanel(Simulator));
            Panels.Add("EditorPowerUps", new PowerUpsAssetsPanel(Simulator));
            Panels.Add("EditorBackground", new BackgroundsAssetsPanel(Simulator));
            Panels.Add("EditorWaves", new WavesPanel(Simulator));
            Panels.Add("EditorPlanetCBAssets", new PlanetCBAssetsPanel(Simulator));
            Panels.Add("EditorPlanetCBAttributes", new PlanetCBAttributesPanel(Simulator));
            Panels.Add("EditorPinkHoleCBAttributes", new PinkHoleCBAttributesPanel(Simulator));
            Panels.Add("EditorEnemies", new EnemiesAssetsPanel(Simulator));
            Panels.Add("EditorWorldName", new WorldNamePanel(Simulator));
            Panels.Add("EditorInfiniteWaves", new InfiniteWavePanel(Simulator));
            Panels.Add("EditorEditWarp", new EditWarpPanel(Simulator));
            Panels.Add("EditorSaveWorld", new SavePanel(Simulator));

            foreach (var p in Panels)
                p.Value.Name = p.Key;
        }
    }
}

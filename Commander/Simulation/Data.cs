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
        public Dictionary<PanelType, Panel> Panels;
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

            Panels = new Dictionary<PanelType, Panel>(PanelTypeComparer.Default);

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

            Panels.Add(PanelType.Credits, new CreditsPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 550), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.GeneralNews, new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.Panel, Color.White, NewsType.General, "What's up at Ephemere Games"));
            Panels.Add(PanelType.UpdatesNews, new NewsPanel(Simulator.Scene, Vector3.Zero, new Vector2(1100, 600), VisualPriorities.Default.Panel, Color.White, NewsType.Updates, "You've just been updated!"));
            Panels.Add(PanelType.Options, new OptionsPanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 400), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.Pause, new PausePanel(Simulator.Scene, Vector3.Zero, new Vector2(400, 600), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.Controls, new ControlsPanel(Simulator.Scene, Vector3.Zero, new Vector2(600, 700), VisualPriorities.Default.Panel, Color.White));
            Panels.Add(PanelType.Help, new HelpPanel(Simulator.Scene, Vector3.Zero, new Vector2(900, 600), VisualPriorities.Default.Panel, Color.White));

            Panels.Add(PanelType.Login, new LoginPanel(Simulator.Scene, Vector3.Zero));
            Panels.Add(PanelType.Register, new RegisterPanel(Simulator.Scene, Vector3.Zero));
            Panels.Add(PanelType.JumpToWorld, new JumpToWorldPanel(Simulator.Scene, Vector3.Zero));
            Panels.Add(PanelType.VirtualKeyboard, new VirtualKeyboardPanel(Simulator.Scene, Vector3.Zero));

            Panels.Add(PanelType.EditorPlayer, new PlayerPanel(Simulator));
            Panels.Add(PanelType.EditorTurrets, new TurretsAssetsPanel(Simulator));
            Panels.Add(PanelType.EditorPowerUps, new PowerUpsAssetsPanel(Simulator));
            Panels.Add(PanelType.EditorBackground, new BackgroundsAssetsPanel(Simulator));
            Panels.Add(PanelType.EditorWaves, new WavesPanel(Simulator));
            Panels.Add(PanelType.EditorCelestialBodyAssets, new CelestialBodyAssetsPanel(Simulator));
            Panels.Add(PanelType.EditorCelestialBodyAttributes, new CelestialBodyAttributesPanel(Simulator));
            Panels.Add(PanelType.EditorEnemies, new EnemiesAssetsPanel(Simulator));
            Panels.Add(PanelType.EditorWorldName, new WorldNamePanel(Simulator));
        }
    }
}

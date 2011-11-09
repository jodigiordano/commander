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
    }
}

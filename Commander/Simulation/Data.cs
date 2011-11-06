namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    
    class Data
    {
        public Level Level;
        public PhysicalRectangle Battlefield;
        public PhysicalRectangle OuterBattlefield;
        public List<Bullet> Bullets;
        public List<ShootingStar> ShootingStars;
        public Path Path;
        public Path PathPreview;
        public List<Enemy> Enemies;

        private Simulator Simulator;


        public Data(Simulator simulator)
        {
            Simulator = simulator;

            Bullets = new List<Bullet>();
            ShootingStars = new List<ShootingStar>();

            Path = new Path(Simulator, new ColorInterpolator(Color.White, Color.Red), 100, BlendType.Add);
            PathPreview = new Path(Simulator, new ColorInterpolator(Color.White, Color.Green), 0, BlendType.Add) { TakeIntoAccountFakeGravTurret = true, TakeIntoAccountFakeGravTurretLv2 = true };

            Enemies = new List<Enemy>();        
        }


        public void Initialize()
        {
            Bullets.Clear();
            ShootingStars.Clear();

            if (Simulator.EditorMode && Simulator.EditorState == EditorState.Editing)
                Battlefield = new PhysicalRectangle(-5000, -5000, 10000, 10000);
            else
                Battlefield = Level.Descriptor.GetBoundaries(new Vector3(6 * (int) Size.Big));

            OuterBattlefield = new PhysicalRectangle(Battlefield.X - 200, Battlefield.Y - 200, Battlefield.Width + 400, Battlefield.Height + 400);

            Level.Initialize();
            Path.Initialize();
            PathPreview.Initialize();

            Enemies.Clear();
        }
    }
}

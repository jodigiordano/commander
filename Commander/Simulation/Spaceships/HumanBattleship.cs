namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HumanBattleship : Spaceship, IVisual
    {
        private List<Turret> Turrets;
        private IDestroyable Target;

        private Particle DieEffect1;
        private Particle DieEffect2;

        private readonly List<Bullet> EmptyList = new List<Bullet>();


        public HumanBattleship(Simulator simulator, string imageName, double visualPriority)
            : base(simulator, imageName)
        {
            Simulator = simulator;

            Image = new Image(imageName)
            {
                VisualPriority = visualPriority,
                SizeX = 4
            };

            Turrets = new List<Turret>();
        }


        public void SetTarget(IDestroyable target)
        {
            Target = target;

            foreach (var t in Turrets)
            {
                t.EnemyWatched = Target;
                t.Watcher = Target == null;
            }
        }


        public override List<Bullet> Fire()
        {
            if (Target == null)
                return EmptyList;

            var bullets = base.Fire();

            foreach (var t in Turrets)
                bullets.AddRange(t.BulletsThisTick());

            return bullets;
        }


        public void AddTurret(Turret t)
        {
            t.VisualPriority = VisualPriority - 0.01;
            t.Position = Position;
            t.EnemyWatched = Target;

            Turrets.Add(t);
        }


        public override Vector3 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;

                if (Turrets != null)
                    foreach (var t in Turrets)
                        t.Position = value + t.RelativePosition;
            }
        }


        public byte Alpha
        {
            get
            {
                return Image.Alpha;
            }
            set
            {
                Image.Alpha = value;

                foreach (var t in Turrets)
                    t.Fade(t.Color.A, value, 0);
            }
        }


        public override void Update()
        {
            base.Update();

            foreach (var t in Turrets)
                t.Update();
        }


        public override void Draw()
        {
            base.Draw();

            foreach (var t in Turrets)
                t.Draw();
        }


        public void DoDie()
        {
            Alive = false;

            DieEffect1 = Simulator.Scene.Particles.Get(@"bouleTerreMeurt");
            DieEffect2 = Simulator.Scene.Particles.Get(Simulator.CutsceneMode ? @"anneauTerreMeurt2" : @"anneauTerreMeurt");

            DieEffect1.VisualPriority = VisualPriority - 0.000001;
            DieEffect2.VisualPriority = VisualPriority - 0.000001;

            var p = Position;

            DieEffect1.Trigger(ref p);
            DieEffect2.Trigger(ref p);

            Simulator.Scene.Particles.Return(DieEffect1);
            Simulator.Scene.Particles.Return(DieEffect2);
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public Vector2 Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Color Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}

namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class TheResistance
    {
        public double ActiveTime;
        public List<Enemy> Enemies;
        private List<Spaceship> Spaceships;


        public TheResistance(Simulator simulator)
        {
            Spaceships = new List<Spaceship>();

            Spaceships.Add(new Spaceship(simulator)
            {
                ShootingFrequency = 100,
                BulletHitPoints = 10,
                MaxRotationRad = 0.15f,
                Image = new Image("Resistance1")
                {
                    SizeX = 4,
                    VisualPriority = VisualPriorities.Default.DefaultSpaceship
                },
                ApplyAutomaticBehavior = true
            });

            Spaceships.Add(new Spaceship(simulator)
            {
                ShootingFrequency = 200,
                BulletHitPoints = 30,
                MaxRotationRad = 0.05f,
                Image = new Image("Resistance2")
                {
                    SizeX = 4,
                    VisualPriority = VisualPriorities.Default.DefaultSpaceship
                },
                ApplyAutomaticBehavior = true
            });

            Spaceships.Add(new Spaceship(simulator)
            {
                ShootingFrequency = 500,
                BulletHitPoints = 100,
                MaxRotationRad = 0.2f,
                Image = new Image("Resistance3")
                {
                    SizeX = 4,
                    VisualPriority = VisualPriorities.Default.DefaultSpaceship
                },
                ApplyAutomaticBehavior = true
            });
        }


        public void Initialize()
        {

        }


        public byte AlphaChannel
        {
            set
            {
                foreach (var spaceship in Spaceships)
                    spaceship.Image.Alpha = value;
            }
        }


        public bool Active
        {
            get { return ActiveTime > 0; }
        }


        public void Update()
        {

        }


        private List<Bullet> projectilesCeTick = new List<Bullet>();
        public List<Bullet> BulletsThisTick()
        {
            projectilesCeTick.Clear();

            foreach (var vaisseau in Spaceships)
                projectilesCeTick.AddRange(vaisseau.BulletsThisTick());

            return projectilesCeTick;
        }


        public void DoHide()
        {

            foreach (var vaisseau in Spaceships)
                vaisseau.DoHide();
        }


        public void Draw()
        {
            foreach (var vaisseau in Spaceships)
                vaisseau.Draw();
        }
    }
}

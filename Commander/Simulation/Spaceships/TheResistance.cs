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
                Image = new Image("Resistance1")
                {
                    SizeX = 4,
                    VisualPriority = VisualPriorities.Default.DefaultSpaceship
                }
            });

            Spaceships.Add(new Spaceship(simulator)
            {
                Image = new Image("Resistance2")
                {
                    SizeX = 4,
                    VisualPriority = VisualPriorities.Default.DefaultSpaceship
                }
            });

            Spaceships.Add(new Spaceship(simulator)
            {
                Image = new Image("Resistance3")
                {
                    SizeX = 4,
                    VisualPriority = VisualPriorities.Default.DefaultSpaceship
                }
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
                projectilesCeTick.AddRange(vaisseau.Fire());

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

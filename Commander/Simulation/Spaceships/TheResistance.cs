namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TheResistance : Spaceship
    {
        public double ActiveTime;
        public List<Enemy> Enemies;
        private List<Spaceship> Spaceships;


        public TheResistance(Simulator simulator)
            : base(simulator)
        {
            Spaceships = new List<Spaceship>();

            Spaceships.Add(new Spaceship(simulator)
            {
                ShootingFrequency = 100,
                BulletHitPoints = 10,
                RotationMaximaleRad = 0.15f,
                Image = new Image("Resistance1")
                {
                    SizeX = 4,
                    VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
                },
                AutomaticMode = true
            });

            Spaceships.Add(new Spaceship(simulator)
            {
                ShootingFrequency = 200,
                BulletHitPoints = 30,
                RotationMaximaleRad = 0.05f,
                Image = new Image("Resistance2")
                {
                    SizeX = 4,
                    VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
                },
                AutomaticMode = true
            });

            Spaceships.Add(new Spaceship(simulator)
            {
                ShootingFrequency = 500,
                BulletHitPoints = 100,
                RotationMaximaleRad = 0.2f,
                Image = new Image("Resistance3")
                {
                    SizeX = 4,
                    VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f
                },
                AutomaticMode = true
            });

            SfxOut = "sfxPowerUpResistanceOut";
            SfxIn = "sfxPowerUpResistanceIn";
        }


        public override void Initialize()
        {
            foreach (var spaceship in Spaceships)
            {
                spaceship.StartingObject = StartingObject;

                if (StartingObject != null)
                    spaceship.Position = StartingObject.Position;
            }
        }


        public byte AlphaChannel
        {
            set
            {
                foreach (var spaceship in Spaceships)
                    spaceship.Image.Color.A = value;
            }
        }


        public override bool Active
        {
            get { return ActiveTime > 0; }
        }


        public override bool TargetReached
        {
            get { return Spaceships[0].TargetReached && Spaceships[1].TargetReached && Spaceships[2].TargetReached; }
        }


        public override void Update()
        {
            ActiveTime -= Preferences.TargetElapsedTimeMs;

            for (int i = 0; i < Spaceships.Count; i++)
            {
                if (!Spaceships[i].InCombat)
                {
                    Vector3 direction = ((Enemies.Count > i) ? Enemies[i].Position : StartingObject.Position) - Spaceships[i].Position;
                    direction.Normalize();

                    Spaceships[i].TargetPosition = ((Enemies.Count > i) ? Enemies[i].Position : StartingObject.Position) + direction * 100;
                }
            }

            foreach (var vaisseau in Spaceships)
            {

                vaisseau.DoAutomaticMode();
                vaisseau.Update();
            }
        }


        private List<Bullet> projectilesCeTick = new List<Bullet>();
        public override List<Bullet> BulletsThisTick()
        {
            projectilesCeTick.Clear();

            foreach (var vaisseau in Spaceships)
                projectilesCeTick.AddRange(vaisseau.BulletsThisTick());

            return projectilesCeTick;
        }


        public override void DoHide()
        {

            foreach (var vaisseau in Spaceships)
            {
                vaisseau.TargetPosition = StartingObject.Position;
                vaisseau.DoHide();
            }
        }


        public override void Draw()
        {
            foreach (var vaisseau in Spaceships)
                vaisseau.Draw();
        }
    }
}

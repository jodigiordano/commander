namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    abstract class SpaceshipWeapon
    {
        protected List<Bullet> Bullets { get; private set; }

        public float AttackPoints;
        public double ShootingFrequency;
        private double LastFireCounter;


        protected Simulator Simulator;
        protected Spaceship Spaceship;


        public SpaceshipWeapon(Simulator simulator, Spaceship spaceship, double frequency, float attackPoints)
        {
            Simulator = simulator;
            Spaceship = spaceship;

            ShootingFrequency = frequency;
            AttackPoints = attackPoints;

            Bullets = new List<Bullet>();
        }


        public void Update()
        {
            LastFireCounter += Preferences.TargetElapsedTimeMs;
        }


        public List<Bullet> Fire()
        {
            Bullets.Clear();

            if (LastFireCounter < ShootingFrequency)
                return Bullets;

            LastFireCounter = 0;

            DoFire();

            foreach (var b in Bullets)
                b.Owner = Spaceship.Id;

            return Bullets;
        }


        public List<Bullet> FireOnceNow()
        {
            DoFire();

            return Bullets;
        }


        protected abstract void DoFire();
    }
}

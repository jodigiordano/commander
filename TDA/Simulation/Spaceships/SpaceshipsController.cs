namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physique;


    class SpaceshipsController
    {
        public List<Enemy> Enemies;
        public List<Mineral> Minerals;
        public HumanBattleship HumanBattleship;
        public event PhysicalObjectHandler ObjectCreated;

        private Simulation Simulation;
        private List<Spaceship> Spaceships;


        public SpaceshipsController(Simulation simulation)
        {
            this.Simulation = simulation;
            this.Spaceships = new List<Spaceship>();
        }


        public void Update()
        {
            for (int i = Spaceships.Count - 1; i > -1; i--)
            {
                var spaceship = Spaceships[i];

                spaceship.Update();

                if (!spaceship.Active && !spaceship.GoBackToStartingObject)
                {
                    spaceship.GoBackToStartingObject = true;
                    spaceship.AutomaticMode = true;
                    spaceship.DoHide();
                }

                if (!spaceship.GoBackToStartingObject)
                {
                    List<Bullet> projectiles = spaceship.BulletsThisTick();

                    for (int j = 0; j < projectiles.Count; j++)
                        NotifyObjectCreated(projectiles[j]);
                }

                if (spaceship.GoBackToStartingObject && spaceship.TargetReached)
                    Spaceships.RemoveAt(i);

                spaceship.NextInput = Vector3.Zero;
            }
        }


        public void Draw()
        {
            foreach (var spaceship in Spaceships)
                spaceship.Draw();
        }


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            switch (powerUp.Type)
            {
                case PowerUpType.Collector: DoAddSpaceshipAsked(((PowerUpCollector) powerUp).Spaceship);                    break;
                case PowerUpType.Spaceship: DoAddSpaceshipAsked(((PowerUpSpaceship) powerUp).Spaceship);                    break;
                case PowerUpType.TheResistance: DoAddSpaceshipAsked(((PowerUpTheResistance) powerUp).Spaceship);            break;
                case PowerUpType.Miner: DoAddSpaceshipAsked(((PowerUpMiner) powerUp).Spaceship);                            break;
                case PowerUpType.AutomaticCollector: DoAddSpaceshipAsked(((PowerUpAutomaticCollector) powerUp).Spaceship);  break;
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            //todo
        }


        public void DoAddSpaceshipAsked(Spaceship spaceship)
        {
            if (spaceship is TheResistance)
                ((TheResistance) spaceship).Enemies = Enemies;

            if (spaceship is SpaceshipAutomaticCollector)
                ((SpaceshipAutomaticCollector) spaceship).Minerals = Minerals;

            spaceship.Initialize();

            Spaceships.Add(spaceship);

            NotifyObjectCreated(spaceship);
        }


        private void NotifyObjectCreated(IObjetPhysique objet)
        {
            if (ObjectCreated != null)
                ObjectCreated(objet);
        }
    }
}

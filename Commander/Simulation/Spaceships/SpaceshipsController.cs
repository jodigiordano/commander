namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;


    class SpaceshipsController
    {
        public List<Mineral> Minerals;
        public PowerUpsBattleship PowerUpsBattleship;
        public event PhysicalObjectHandler ObjectCreated;
        public event PhysicalObjectHandler ObjectDestroyed;

        private Simulator Simulator;
        private List<Spaceship> Spaceships;
        private Dictionary<SimPlayer, Spaceship> Players;


        public SpaceshipsController(Simulator simulator)
        {
            Simulator = simulator;
            Spaceships = new List<Spaceship>();
            Players = new Dictionary<SimPlayer, Spaceship>();
        }


        public void DoPlayerConnected(SimPlayer player)
        {
            Players.Add(player, player.SpaceshipMove);

            NotifyObjectCreated(player.SpaceshipMove);
        }


        public void DoPlayerDisconnected(SimPlayer player)
        {
            Players.Remove(player);

            NotifyObjectDestroyed(player.SpaceshipMove);
        }


        public void Update()
        {
            for (int i = Spaceships.Count - 1; i > -1; i--)
            {
                var spaceship = Spaceships[i];

                spaceship.Update();

                if (spaceship.SteeringBehavior is SpaceshipGoHomeABehavior)
                {
                    if (!spaceship.SteeringBehavior.Active)
                        Spaceships.RemoveAt(i);
                }

                else
                {
                    if (!spaceship.Active)
                    {
                        spaceship.SteeringBehavior = new SpaceshipGoHomeABehavior(spaceship);
                        spaceship.DoHide();
                    }

                    else
                    {
                        List<Bullet> bullets = spaceship.Fire();

                        for (int j = 0; j < bullets.Count; j++)
                            NotifyObjectCreated(bullets[j]);
                    }
                }
            }
        }


        public void Draw()
        {
            foreach (var spaceship in Spaceships)
                spaceship.Draw();
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            switch (powerUp.Type)
            {
                case PowerUpType.Collector: DoAddSpaceshipAsked(((PowerUpCollector) powerUp).Collector);                break;
                case PowerUpType.Spaceship: DoAddSpaceshipAsked(((PowerUpSpaceship) powerUp).Spaceship);       break;
                case PowerUpType.Miner: DoAddSpaceshipAsked(((PowerUpMiner) powerUp).Miner);                            break;
                case PowerUpType.AutomaticCollector: DoAddSpaceshipAsked(((PowerUpAutomaticCollector) powerUp).AutomaticCollector);  break;
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp, SimPlayer player)
        {
            //todo
        }


        public void DoAddSpaceshipAsked(Spaceship spaceship)
        {
            if (spaceship is SpaceshipAutomaticCollector)
                ((SpaceshipAutomaticCollector) spaceship).Minerals = Minerals;

            spaceship.Initialize();

            Spaceships.Add(spaceship);

            NotifyObjectCreated(spaceship);
        }


        public void DoShieldCollided(ICollidable collidable, Bullet b)
        {
            var spaceship = collidable as Spaceship;

            if (spaceship == null)
                return;

            spaceship.DoShieldHit(b.Position);
        }


        private void NotifyObjectCreated(ICollidable objet)
        {
            if (ObjectCreated != null)
                ObjectCreated(objet);
        }


        private void NotifyObjectDestroyed(ICollidable obj)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(obj);
        }
    }
}

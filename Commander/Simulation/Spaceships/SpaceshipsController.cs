﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class SpaceshipsController
    {
        public List<Enemy> Enemies;
        public List<Mineral> Minerals;
        public HumanBattleship HumanBattleship;
        public event PhysicalObjectHandler ObjectCreated;

        private Simulator Simulator;
        private List<Spaceship> Spaceships;


        public SpaceshipsController(Simulator simulator)
        {
            this.Simulator = simulator;
            this.Spaceships = new List<Spaceship>();
        }


        public void Update()
        {
            for (int i = Spaceships.Count - 1; i > -1; i--)
            {
                var spaceship = Spaceships[i];

                spaceship.Update();

                if (spaceship.AutomaticBehavior is SpaceshipGoHomeBehavior)
                {
                    if (!spaceship.AutomaticBehavior.Active)
                        Spaceships.RemoveAt(i);
                }

                else
                {
                    if (!spaceship.Active)
                    {
                        spaceship.AutomaticBehavior = new SpaceshipGoHomeBehavior(spaceship);
                        spaceship.ApplyAutomaticBehavior = true;
                        spaceship.DoHide();
                    }

                    else
                    {
                        List<Bullet> bullets = spaceship.BulletsThisTick();

                        for (int j = 0; j < bullets.Count; j++)
                            NotifyObjectCreated(bullets[j]);
                    }
                }

                spaceship.NextMovement = Vector3.Zero;
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
                case PowerUpType.Spaceship: DoAddSpaceshipAsked(((PowerUpSpaceship) powerUp).SpaceshipSpaceship);       break;
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
            //if (spaceship is TheResistance)
            //    ((TheResistance) spaceship).Enemies = Enemies;

            if (spaceship is SpaceshipAutomaticCollector)
                ((SpaceshipAutomaticCollector) spaceship).Minerals = Minerals;

            spaceship.Initialize();

            Spaceships.Add(spaceship);

            NotifyObjectCreated(spaceship);
        }


        private void NotifyObjectCreated(ICollidable objet)
        {
            if (ObjectCreated != null)
                ObjectCreated(objet);
        }
    }
}

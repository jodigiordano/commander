namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;


    class BulletsController
    {
        public event PhysicalObjectHandler ObjectDestroyed;
        public List<Projectile> Bullets { get; private set; }

        private Simulation Simulation;


        public BulletsController(Simulation simulation)
        {
            Simulation = simulation;

            Bullets = new List<Projectile>();
        }


        public void Update()
        {
            for (int i = Bullets.Count - 1; i > -1; i--)
                if (!Bullets[i].Alive)
                {
                    Bullets[i].DoDie();
                    Bullets.RemoveAt(i);
                }

            for (int i = 0; i < Bullets.Count; i++)
                Bullets[i].Update();
        }


        public void Draw()
        {
            for (int i = 0; i < Bullets.Count; i++)
                Bullets[i].Draw();
        }


        public void DoObjectCreated(IObjetPhysique objet)
        {
            if (!(objet is Projectile))
                return;

            Bullets.Add((Projectile) objet);
        }


        private void NotifyObjectDestroyed(Projectile bullet)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(bullet);
        }


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp.Type == PowerUpType.Pulse)
                Bullets.Add(((PowerUpPulse) powerUp).Bullet);
            else if (powerUp.Type == PowerUpType.Shield)
                Bullets.Add(((PowerUpShield) powerUp).Bullet);
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            if (powerUp.Type == PowerUpType.Pulse)
                ((PowerUpPulse) powerUp).Bullet.LifePoints = 0;
            else if (powerUp.Type == PowerUpType.Shield)
                ((PowerUpShield) powerUp).Bullet.LifePoints = 0;
        }
    }
}

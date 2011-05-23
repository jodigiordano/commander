namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PowerUpsController
    {
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public event PowerUpHandler PowerUpStarted;
        public event PowerUpHandler PowerUpStopped;

        private List<PowerUp> PowerUps;
        private Simulation Simulation;


        public PowerUpsController(Simulation simulation)
        {
            Simulation = simulation;

            ActivesPowerUps = new Dictionary<PowerUpType, bool>();
            PowerUps = new List<PowerUp>();
        }


        public void Initialize()
        {
            foreach (var powerUp in Simulation.PowerUpsFactory.Availables.Keys)
                ActivesPowerUps.Add(powerUp, true);
        }


        public void Update()
        {
            for (int i = PowerUps.Count - 1; i > -1; i--)
                if (PowerUps[i].Terminated)
                {
                    PowerUp p = PowerUps[i];

                    ActivesPowerUps[p.Type] = true;

                    PowerUps.RemoveAt(i);

                    NotifyPowerUpStopped(p);
                }
        }


        public void DoBuyAPowerUpAsked(PowerUpType type)
        {
            ActivesPowerUps[type] = false;

            PowerUp p = Simulation.PowerUpsFactory.Create(type);

            p.Start();

            PowerUps.Add(p);

            NotifyPowerUpStarted(p);
        }


        private void NotifyPowerUpStarted(PowerUp powerUp)
        {
            if (PowerUpStarted != null)
                PowerUpStarted(powerUp);
        }


        private void NotifyPowerUpStopped(PowerUp powerUp)
        {
            if (PowerUpStopped != null)
                PowerUpStopped(powerUp);
        }
    }
}

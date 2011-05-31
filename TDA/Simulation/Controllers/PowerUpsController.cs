﻿namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PowerUpsController
    {
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public event PowerUpHandler PowerUpStarted;
        public event PowerUpHandler PowerUpStopped;
        public bool InPowerUp;

        private List<PowerUp> PowerUps;
        private Simulation Simulation;


        public PowerUpsController(Simulation simulation)
        {
            Simulation = simulation;

            ActivesPowerUps = new Dictionary<PowerUpType, bool>();
            PowerUps = new List<PowerUp>();
            InPowerUp = false;
        }


        public void Initialize()
        {
            foreach (var powerUp in Simulation.PowerUpsFactory.Availables.Keys)
                ActivesPowerUps.Add(powerUp, true);
        }


        public void Update()
        {
            for (int i = PowerUps.Count - 1; i > -1; i--)
            {
                PowerUp p = PowerUps[i];

                p.Update();

                if (PowerUps[i].Terminated)
                {
                    ActivesPowerUps[p.Type] = true;

                    PowerUps.RemoveAt(i);

                    if (p.NeedInput)
                        InPowerUp = false;

                    NotifyPowerUpStopped(p);
                }
            }
        }


        public void DoBuyAPowerUpAsked(PowerUpType type)
        {
            ActivesPowerUps[type] = false;

            PowerUp p = Simulation.PowerUpsFactory.Create(type);

            p.Start();

            PowerUps.Add(p);

            if (p.NeedInput)
                InPowerUp = true;

            NotifyPowerUpStarted(p);
        }


        public void DoPlayerMoved(SimPlayer player)
        {
            foreach (var powerUp in PowerUps)
                if (powerUp.NeedInput)
                    powerUp.DoInputMoved(player.Position);
        }


        public void DoInputCanceled()
        {
            foreach (var powerUp in PowerUps)
                if (powerUp.NeedInput)
                    powerUp.DoInputCanceled();
        }


        public void DoInputReleased()
        {
            foreach (var powerUp in PowerUps)
                if (powerUp.NeedInput)
                    powerUp.DoInputReleased();
        }


        public void DoInputPressed()
        {
            foreach (var powerUp in PowerUps)
                if (powerUp.NeedInput)
                    powerUp.DoInputPressed();
        }


        public void DoInputMovedDelta(Vector3 delta)
        {
            foreach (var powerUp in PowerUps)
                if (powerUp.NeedInput)
                    powerUp.DoInputMovedDelta(delta);
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

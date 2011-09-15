namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PowerUpsController
    {
        public Dictionary<PowerUpType, bool> ActivesPowerUps;
        public event PowerUpSimPlayerHandler PowerUpStarted;
        public event PowerUpSimPlayerHandler PowerUpStopped;
        public event PowerUpSimPlayerHandler PowerUpInputCanceled;
        public event PowerUpSimPlayerHandler PowerUpInputReleased;
        public event PowerUpSimPlayerHandler PowerUpInputPressed;

        private Dictionary<SimPlayer, List<PowerUp>> PowerUps;
        private Simulator Simulator;


        public PowerUpsController(Simulator simulator)
        {
            Simulator = simulator;

            ActivesPowerUps = new Dictionary<PowerUpType, bool>();
            PowerUps = new Dictionary<SimPlayer, List<PowerUp>>();
        }


        public void Initialize()
        {
            ActivesPowerUps.Clear();

            foreach (var powerUp in Simulator.PowerUpsFactory.Availables.Keys)
                ActivesPowerUps.Add(powerUp, true);
        }


        public void DoPlayerConnected(SimPlayer player)
        {
            if (!PowerUps.ContainsKey(player))
                PowerUps.Add(player, new List<PowerUp>());
        }


        public void DoPlayerDisconnected(SimPlayer player)
        {
            foreach (var powerUp in PowerUps[player])
                powerUp.TerminatedOverride = true;
        }


        public void Update()
        {
            foreach (var kvp in PowerUps)
            {
                var player = kvp.Key;
                var pus = kvp.Value;

                for (int i = pus.Count - 1; i > -1; i--)
                {
                    PowerUp p = pus[i];

                    p.Update();

                    if (pus[i].Terminated)
                    {
                        p.Stop();

                        ActivesPowerUps[p.Type] = true;

                        pus.RemoveAt(i);

                        if (p.NeedInput)
                            player.PowerUpInUse = PowerUpType.None;

                        NotifyPowerUpStopped(p, player);
                    }
                }
            }
        }


        public void DoActivatePowerUpAsked(PowerUpType type, SimPlayer player)
        {
            ActivesPowerUps[type] = false;

            PowerUp p = Simulator.PowerUpsFactory.Create(type);

            p.Owner = player;
            p.Start();

            PowerUps[player].Add(p);

            if (p.NeedInput)
                player.PowerUpInUse = p.Type;

            NotifyPowerUpStarted(p, player);
        }


        public void DoDesactivatePowerUpAsked(PowerUpType type, SimPlayer player)
        {
            PowerUp p = null;
            
            foreach (var powerUp in PowerUps[player])
                if (powerUp.Type == type)
                {
                    p = powerUp;
                    break;
                }

            p.Stop();

            ActivesPowerUps[p.Type] = true;

            PowerUps[player].Remove(p);

            if (p.NeedInput)
                player.PowerUpInUse = PowerUpType.None;

            NotifyPowerUpStopped(p, player);
        }


        public void DoPlayerMoved(SimPlayer player)
        {
            foreach (var powerUp in PowerUps[player])
                if (powerUp.NeedInput)
                    powerUp.DoInputMoved(player.Position);
        }


        public void DoInputCanceled(SimPlayer player)
        {
            foreach (var powerUp in PowerUps[player])
                if (powerUp.NeedInput)
                {
                    powerUp.DoInputCanceled();
                    NotifyPowerUpInputCanceled(powerUp, player);
                }
        }


        public void DoInputReleased(SimPlayer player)
        {
            foreach (var powerUp in PowerUps[player])
                if (powerUp.NeedInput)
                {
                    powerUp.DoInputReleased();
                    NotifyPowerUpInputReleased(powerUp, player);
                }
        }


        public void DoInputPressed(SimPlayer player)
        {
            foreach (var powerUp in PowerUps[player])
                if (powerUp.NeedInput)
                {
                    powerUp.DoInputPressed();
                    NotifyPowerUpInputPressed(powerUp, player);
                }
        }


        public void DoPlayerMovedDelta(SimPlayer player, Vector3 delta)
        {
            foreach (var powerUp in PowerUps[player])
                if (powerUp.NeedInput)
                    powerUp.DoInputMovedDelta(delta);
        }


        public void DoNewGameState(GameState state)
        {
            if (state == GameState.Lost || state == GameState.Won)
            {
                foreach (var kvp in PowerUps)
                {
                    kvp.Key.PowerUpInUse = PowerUpType.None;

                    foreach (var p in kvp.Value)
                        p.TerminatedOverride = true;
                }
            }
        }


        public void DoEditorCommandExecuted(EditorCommand command)
        {
            if (command.Name == "AddOrRemovePowerUp")
                Initialize();
        }


        private void NotifyPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            if (PowerUpStarted != null)
                PowerUpStarted(powerUp, player);
        }


        private void NotifyPowerUpStopped(PowerUp powerUp, SimPlayer player)
        {
            if (PowerUpStopped != null)
                PowerUpStopped(powerUp, player);
        }


        private void NotifyPowerUpInputCanceled(PowerUp powerUp, SimPlayer player)
        {
            if (PowerUpInputCanceled != null)
                PowerUpInputCanceled(powerUp, player);
        }


        private void NotifyPowerUpInputReleased(PowerUp powerUp, SimPlayer player)
        {
            if (PowerUpInputReleased != null)
                PowerUpInputReleased(powerUp, player);
        }


        private void NotifyPowerUpInputPressed(PowerUp powerUp, SimPlayer player)
        {
            if (PowerUpInputPressed != null)
                PowerUpInputPressed(powerUp, player);
        }
    }
}

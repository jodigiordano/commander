namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.XACTAudio;


    class AudioController
    {
        public EnemiesData EnemiesData;
        public CelestialBody CelestialBodyToProtect;

        private Simulator Simulator;
        private Dictionary<SimPlayer, AudioPlayer> Players;
        private AudioTurretsController AudioTurretsController;
        private Cue WaveNearToStartCue;


        public AudioController(Simulator simulator)
        {
            Simulator = simulator;
            AudioTurretsController = new AudioTurretsController();
            Players = new Dictionary<SimPlayer, AudioPlayer>();
        }


        public void Initialize()
        {
            XACTAudio.SetGlobalVariable("DistanceFromPlanet", 0);
            XACTAudio.SetGlobalVariable("NumberOfAliens", 0);
        }


        public void Update()
        {
            if (EnemiesData.EnemyCountPercChanged)
                XACTAudio.SetGlobalVariable("NumberOfAliens", (float) EnemiesData.EnemyCountPerc);

            if (EnemiesData.EnemyNearHitPercChanged)
                XACTAudio.SetGlobalVariable("DistanceFromPlanet", (float) EnemiesData.EnemyNearHitPerc);

            // Deleted sound effects (need to be hooked again)
            // - Typewriter
            // - Pulse power-up

            foreach (var p in Players.Values)
                p.Update();

            AudioTurretsController.Update();
        }


        public void DoObjectHit(ICollidable obj)
        {
            var celestialBody = obj as CelestialBody;

            if (celestialBody != null)
            {
                if (celestialBody == CelestialBodyToProtect)
                    XACTAudio.PlayCue("PlanetToProtectHit", "Sound Bank");
                else
                    XACTAudio.PlayCue("SilentCue" /*"PlanetHit"*/, "Sound Bank");

                return;
            }


            var enemy = obj as Enemy;

            if (enemy != null)
            {
                XACTAudio.PlayCue(enemy.SfxHit, "Sound Bank");

                return;
            }
        }


        public void DoObjectDestroyed(ICollidable obj)
        {
            var bullet = obj as Bullet;

            if (bullet != null)
            {
                if (Simulator.State != GameState.Running || Simulator.CutsceneMode)
                    return;

                if (!bullet.OutOfBounds && bullet.Explosive && bullet.SfxExplosion != "")
                    XACTAudio.PlayCue(bullet.SfxExplosion, "Sound Bank");

                return;
            }


            var celestialBody = obj as CelestialBody;

            if (celestialBody != null)
            {
                if (celestialBody.SilentDeath)
                    return;

                if (celestialBody == CelestialBodyToProtect)
                    XACTAudio.PlayCue("PlanetToProtectDestroyed", "Sound Bank");
                else
                    XACTAudio.PlayCue("PlanetDestroyed", "Sound Bank");

                return;
            }


            var enemy = obj as Enemy;

            if (enemy != null)
            {
                if (Simulator.State != GameState.Running)
                    return;

                XACTAudio.PlayCue(enemy.SfxDie, "Sound Bank");

                return;
            }


            var mineral = obj as Mineral;

            if (mineral != null)
            {
                XACTAudio.PlayCue("SilentCue" /*mineral.SfxDie*/, "Sound Bank");

                return;
            }
        }


        public void DoWaveNearToStart()
        {
            if (Simulator.DemoMode)
                return;

            WaveNearToStartCue = XACTAudio.GetCue("NewWaveComing", "Sound Bank");
            WaveNearToStartCue.Play();
        }


        public void DoWaveStarted()
        {
            if (Simulator.DemoMode)
                return;

            if (WaveNearToStartCue != null)
            {
                WaveNearToStartCue.Stop();
                WaveNearToStartCue = null;
            }

            XACTAudio.PlayCue("NewWaveLaunching", "Sound Bank");
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
        {
            XACTAudio.PlayCue("SilentCue" /*powerUp.SfxIn*/, "Sound Bank");
        }


        public void DoPowerUpStopped(PowerUp powerUp, SimPlayer player)
        {
            XACTAudio.PlayCue("SilentCue" /*powerUp.SfxOut*/, "Sound Bank");
        }


        public void DoTurretBoughtStarted(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            AudioTurretsController.AddTurretBought(turret);
            //if (turret.Type == TurretType.Gravitational)
            //    XACTAudio.PlayCue("GravitationalTurretBought", "Sound Bank");
            //else
            //    XACTAudio.PlayCue("TurretBought", "Sound Bank");
        }


        public void DoTurretUpgradingStarted(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            AudioTurretsController.AddTurretUpgraded(turret);
            //if (turret.Type == TurretType.Gravitational)
            //    XACTAudio.PlayCue("GravitationalTurretUpgrade", "Sound Bank");
            //else
            //    XACTAudio.PlayCue("TurretUpgraded", "Sound Bank");
        }


        public void DoTurretSold(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            XACTAudio.PlayCue("TurretSold", "Sound Bank");
        }


        public void DoTurretFired(Turret turret)
        {
            if (Simulator.State != GameState.Running)
                return;

            if (turret.Type == TurretType.Basic || turret.Type == TurretType.Laser ||
                turret.Type == TurretType.Missile || turret.Type == TurretType.MultipleLasers ||
                turret.Type == TurretType.SlowMotion)
                XACTAudio.PlayCue(turret.FiringSfx, "Sound Bank");
        }


        //public void DoTurretReactivated(Turret turret)
        //{
        //    if (!turret.BackActiveThisTickOverride)
        //        XACTAudio.PlayCue("TurretUpgraded", "Sound Bank");
        //}


        public void DoTurretWandered(Turret turret)
        {
            XACTAudio.PlayCue("SilentCue" /*turret.MovingSfx*/, "Sound Bank");
        }


        public void DoPowerUpInputCanceled(PowerUp powerUp, SimPlayer player)
        {

        }


        public void DoPowerUpInputReleased(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.Miner)
                XACTAudio.PlayCue("SilentCue" /*"sfxMineGround"*/, "Sound Bank");
        }


        public void DoPowerUpInputPressed(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.RailGun)
                XACTAudio.PlayCue("SilentCue" /*"sfxRailGunCharging"*/, "Sound Bank");
        }


        public void DoStartingPathCollision(Bullet b, CelestialBody cb)
        {
            XACTAudio.PlayCue("MothershipShieldHit", "Sound Bank");
        }


        public void DoPlayersCollided(SimPlayer p1, SimPlayer p2)
        {
            XACTAudio.PlayCue("ShipToShipCollision", "Sound Bank");
        }


        public void DoPlayerBounced(SimPlayer player)
        {
            XACTAudio.PlayCue("ShipBouncing", "Sound Bank");
        }


        public void DoBulletDeflected(Bullet b)
        {
            XACTAudio.PlayCue("VulcanoidDeflection", "Sound Bank");
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            var audioPlayer = new AudioPlayer(p);

            Players.Add(p, audioPlayer);

            audioPlayer.TeleportIn();
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            var audioPlayer = Players[p];

            audioPlayer.StopLoopingCues();
            audioPlayer.TeleportOut();

            Players.Remove(p);
        }


        public void DoPlayerFired(SimPlayer p)
        {
            Players[p].Fire();
        }


        public void TeleportPlayers(bool teleportOut)
        {
            // for now only one sound effect, could be nice to have a sfx for multiple ships
            XACTAudio.PlayCue(teleportOut ? "ShipTeleportOut" : "ShipTeleportIn", "Sound Bank");
        }


        public void DoWaveEnded()
        {
            if (Simulator.State == GameState.Running || Simulator.State == GameState.Won)
                XACTAudio.PlayCue("WaveDestroyed", "Sound Bank");
        }


        public void DoNewGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    PauseLoopingCues();
                    break;

                case GameState.Running:
                    ResumeLoopingCues();
                    break;
            }
        }


        public void DoFocusLost()
        {
            PauseLoopingCues();
        }


        public void DoFocusGained()
        {
            ResumeLoopingCues();
        }


        public void DoPlayerSelectionChanged(SimPlayer p)
        {
            AudioPlayer player = Players[p];

            if (p.ActualSelection.CelestialBody != p.LastSelection.CelestialBody)
            {
                if (p.ActualSelection.CelestialBody != null && p.ActualSelection.CelestialBody.FirstOnPath)
                    player.StartOnMothership();
                else if (p.ActualSelection.CelestialBody == null && p.LastSelection.CelestialBody != null && p.LastSelection.CelestialBody.FirstOnPath)
                    player.StopOnMothership();
                else if (p.ActualSelection.CelestialBody != null)
                    player.StartOnCelestialBody();
                else if (p.ActualSelection.CelestialBody == null)
                    player.StopOnCelestialBody();
            }

            if (Simulator.DemoMode)
            {
                if (p.ActualSelection.NewGameChoiceChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");

                if (Simulator.WorldMode)
                {
                    if (p.ActualSelection.PausedGameChoiceChanged)
                        XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");
                }
            }

            else if (Simulator.EditorMode)
            {
            
            }

            else
            {
                // Power-Ups
                if (p.ActualSelection.PowerUpToBuyChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");

                // Turrets
                if (p.ActualSelection.TurretToPlaceChanged && p.ActualSelection.TurretToPlace != null)
                    player.StartInstallingTurret();
                else if (p.ActualSelection.TurretToPlaceChanged && p.ActualSelection.TurretToPlace == null)
                    player.StopInstallingTurret();

                if (p.ActualSelection.TurretToBuyChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");

                // Turret
                if (p.ActualSelection.TurretChoiceChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");
            }
        }


        public void DoPlayerActionRefused(SimPlayer player)
        {
            XACTAudio.PlayCue("ContextualMenuSelectionError", "Sound Bank");
        }


        public void DoPanelOpened()
        {
            XACTAudio.PlayCue("PanelOpen", "Sound Bank");
        }


        public void DoPanelClosed()
        {
            XACTAudio.PlayCue("PanelClose", "Sound Bank");
        }


        private void PauseLoopingCues()
        {
            foreach (var p in Players.Values)
                p.PauseLoopingCues();

            if (WaveNearToStartCue != null)
                WaveNearToStartCue.Pause();

            AudioTurretsController.PauseLoopingCues();
        }


        private void ResumeLoopingCues()
        {
            foreach (var p in Players.Values)
                p.ResumeLoopingCues();

            if (WaveNearToStartCue != null)
                WaveNearToStartCue.Resume();

            AudioTurretsController.ResumeLoopingCues();
        }
    }
}

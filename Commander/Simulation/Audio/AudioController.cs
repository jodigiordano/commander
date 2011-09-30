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

        private Cue WaveNearToStartCue;


        public AudioController(Simulator simulator)
        {
            Simulator = simulator;
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
            // - Player(s) firing
            // - Typewriter
            // - Pulse power-up

            foreach (var p in Players.Values)
                p.Update();
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
                if (!bullet.OutOfBounds && !Simulator.CutsceneMode && bullet.Explosive && bullet.SfxExplosion != "")
                    XACTAudio.PlayCue("SilentCue" /*bullet.SfxExplosion*/, "Sound Bank");

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
                if (Simulator.State == GameState.Running)
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
                WaveNearToStartCue.Stop();

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


        public void DoTurretBought(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            if (turret.Type == TurretType.Gravitational)
                XACTAudio.PlayCue("GravitationalTurretBought", "Sound Bank");
            else
                XACTAudio.PlayCue("TurretBought", "Sound Bank");
        }


        public void DoTurretUpgraded(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            if (turret.Type == TurretType.Gravitational)
                XACTAudio.PlayCue("GravitationalTurretUpgrade", "Sound Bank");
            else
                XACTAudio.PlayCue("TurretUpgraded", "Sound Bank");
        }


        public void DoTurretSold(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            XACTAudio.PlayCue("TurretSold", "Sound Bank");
        }


        public void DoTurretFired(Turret turret)
        {
            if (turret.Type == TurretType.Basic || turret.Type == TurretType.Laser ||
                turret.Type == TurretType.Missile || turret.Type == TurretType.MultipleLasers)
                XACTAudio.PlayCue(turret.FiringSfx, "Sound Bank");
        }


        public void DoTurretReactivated(Turret turret)
        {
            if (!turret.BackActiveThisTickOverride)
                XACTAudio.PlayCue("TurretUpgraded", "Sound Bank");
        }


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
            XACTAudio.PlayCue("SilentCue" /*"ShipToShipCollision"*/, "Sound Bank");
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
            if (Simulator.State == GameState.Running)
                XACTAudio.PlayCue("WaveDestroyed", "Sound Bank");
        }


        public void DoNewGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    foreach (var p in Players.Values)
                        p.PauseLoopingCues();
                    break;

                case GameState.Running:
                    foreach (var p in Players.Values)
                        p.ResumeLoopingCues();
                    break;
            }
        }


        public void DoFocusLost()
        {
            foreach (var p in Players.Values)
                p.PauseLoopingCues();
        }


        public void DoFocusGained()
        {
            foreach (var p in Players.Values)
                p.ResumeLoopingCues();
        }


        public void DoPlayerSelectionChanged(SimPlayer p)
        {
            if (Simulator.DemoMode)
            {
                if (p.ActualSelection.NewGameChoiceChanged && p.LastSelection.NewGameChoice == NewGameChoice.None)
                    XACTAudio.PlayCue("ContextualMenuIn", "Sound Bank");
                else if (p.ActualSelection.NewGameChoiceChanged && p.LastSelection.NewGameChoice != NewGameChoice.None && p.ActualSelection.NewGameChoice == NewGameChoice.None)
                { /*XACTAudio.PlayCue("ContextualMenuOut", "Sound Bank");*/ }
                else if (p.ActualSelection.NewGameChoiceChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");

                if (Simulator.WorldMode)
                {
                    if (p.ActualSelection.PausedGameChoiceChanged && p.LastSelection.PausedGameChoice == PausedGameChoice.None)
                        XACTAudio.PlayCue("ContextualMenuIn", "Sound Bank");
                    else if (p.ActualSelection.PausedGameChoiceChanged && p.LastSelection.PausedGameChoice != PausedGameChoice.None && p.ActualSelection.PausedGameChoice == PausedGameChoice.None)
                    { /*XACTAudio.PlayCue("ContextualMenuOut", "Sound Bank");*/ }
                    else if (p.ActualSelection.PausedGameChoiceChanged)
                        XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");
                }
            }

            else if (Simulator.EditorMode)
            {
            
            }

            else
            {
                // Power-Ups
                if (p.ActualSelection.PowerUpToBuyChanged && p.LastSelection.PowerUpToBuy == PowerUpType.None)
                    XACTAudio.PlayCue("ContextualMenuIn", "Sound Bank");
                else if (p.ActualSelection.PowerUpToBuyChanged && p.LastSelection.PowerUpToBuy != PowerUpType.None && p.ActualSelection.PowerUpToBuy == PowerUpType.None)
                { /*XACTAudio.PlayCue("ContextualMenuOut", "Sound Bank");*/ }
                else if (p.ActualSelection.PowerUpToBuyChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");

                // Turrets
                if (p.ActualSelection.TurretToBuyChanged && p.LastSelection.TurretToBuy == TurretType.None)
                    XACTAudio.PlayCue("ContextualMenuIn", "Sound Bank");
                else if (p.ActualSelection.TurretToBuyChanged && p.LastSelection.TurretToBuy != TurretType.None && p.ActualSelection.TurretToBuy == TurretType.None)
                { /*XACTAudio.PlayCue("ContextualMenuOut", "Sound Bank");*/ }
                else if (p.ActualSelection.TurretToBuyChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");

                // Turret
                if (p.ActualSelection.TurretChoiceChanged && p.LastSelection.TurretChoice == TurretChoice.None)
                    XACTAudio.PlayCue("ContextualMenuIn", "Sound Bank");
                else if (p.ActualSelection.TurretChoiceChanged && p.LastSelection.TurretChoice != TurretChoice.None && p.ActualSelection.TurretChoice == TurretChoice.None)
                { /*XACTAudio.PlayCue("ContextualMenuOut", "Sound Bank");*/ }
                else if (p.ActualSelection.TurretChoiceChanged)
                    XACTAudio.PlayCue("ContextualMenuSelectionChange", "Sound Bank");
            }
        }


        public void DoPlayerActionRefused(SimPlayer player)
        {
            XACTAudio.PlayCue("ContextualMenuSelectionError", "Sound Bank");
        }
    }
}

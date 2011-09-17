namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.XACTAudio;


    class AudioController
    {
        public EnemiesData EnemiesData;
        public CelestialBody CelestialBodyToProtect;

        private Simulator Simulator;

        public AudioController(Simulator simulator)
        {
            Simulator = simulator;
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
                if (enemy.Type == EnemyType.Centaur || enemy.Type == EnemyType.Comet || enemy.Type == EnemyType.Damacloid)
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
                if (enemy.Type != EnemyType.Plutoid && enemy.Type != EnemyType.Meteoroid)
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
            if (!Simulator.DemoMode)
                XACTAudio.PlayCue("NewWaveComing", "Sound Bank");
        }


        public void DoWaveStarted()
        {
            if (!Simulator.DemoMode)
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
                XACTAudio.PlayCue("SilentCue" /*turret.FiringSfx*/, "Sound Bank");
            else
                XACTAudio.PlayCue("TurretBought", "Sound Bank");
        }


        public void DoTurretUpgraded(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            XACTAudio.PlayCue("SilentCue" /*TurretUpgraded*/, "Sound Bank");
        }


        public void DoTurretSold(Turret turret, SimPlayer player)
        {
            if (Simulator.DemoMode)
                return;

            XACTAudio.PlayCue("SilentCue" /*"TurretSold"*/, "Sound Bank");
        }


        public void DoTurretFired(Turret turret)
        {
            if (turret.Type == TurretType.Basic || turret.Type == TurretType.Laser || turret.Type == TurretType.Missile)
                Core.XACTAudio.XACTAudio.PlayCue(turret.FiringSfx, "Sound Bank");
        }


        public void DoTurretReactivated(Turret turret)
        {
            if (!turret.BackActiveThisTickOverride)
                Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"sfxTourelleMiseAJour"*/, "Sound Bank");
        }


        public void DoTurretWandered(Turret turret)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*turret.MovingSfx*/, "Sound Bank");
        }


        public void DoPowerUpInputCanceled(PowerUp powerUp, SimPlayer player)
        {

        }


        public void DoPowerUpInputReleased(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.Miner)
                Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"sfxMineGround"*/, "Sound Bank");
        }


        public void DoPowerUpInputPressed(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type == PowerUpType.RailGun)
                Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"sfxRailGunCharging"*/, "Sound Bank");
        }


        public void DoStartingPathCollision(Bullet b, CelestialBody cb)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"MothershipShieldHit"*/, "Sound Bank");
        }


        public void DoPlayersCollided(SimPlayer p1, SimPlayer p2)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"ShipToShipCollision"*/, "Sound Bank");
        }


        public void DoPlayerBounced(SimPlayer player)
        {
            Core.XACTAudio.XACTAudio.PlayCue("ShipBouncing", "Sound Bank");
        }


        public void DoBulletDeflected(Bullet b)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"VulcanoidDeflection"*/, "Sound Bank");
        }


        public void DoPlayerConnected(SimPlayer p)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"ShipTeleportIn"*/, "Sound Bank");
        }


        public void DoPlayerDisconnected(SimPlayer p)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"ShipTeleportOut"*/, "Sound Bank");
        }


        public void TeleportPlayers(bool teleportOut)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*teleportOut ? "ShipTelportOut" : "ShipTelportIn"*/, "Sound Bank");
        }


        public void DoWaveEnded()
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"WaveDestroyed"*/, "Sound Bank");
        }
    }
}

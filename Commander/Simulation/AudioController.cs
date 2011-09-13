namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.XACTAudio;


    class AudioController
    {
        public EnemiesData EnemiesData;

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
            XACTAudio.SetGlobalVariable("NumberOfAliens", (float) EnemiesData.EnemyCountPerc);
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
                XACTAudio.PlayCue("SilentCue" /*"sfxCorpsCelesteTouche"*/, "Sound Bank");

                return;
            }
        }


        public void DoObjectDestroyed(ICollidable obj)
        {
            var bullet = obj as Bullet;

            if (bullet != null)
            {
                if (!bullet.OutOfBounds && !Simulator.CutsceneMode && bullet.SfxExplosion != "")
                    XACTAudio.PlayCue("SilentCue" /*bullet.SfxExplosion*/, "Sound Bank");

                return;
            }


            var celestialBody = obj as CelestialBody;

            if (celestialBody != null)
            {
                if (!celestialBody.SilentDeath)
                    XACTAudio.PlayCue("SilentCue" /*"sfxCorpsCelesteExplose"*/, "Sound Bank");

                return;
            }


            var mineral = obj as Mineral;

            if (mineral != null)
            {
                XACTAudio.PlayCue("SilentCue" /*mineral.SfxDie*/, "Sound Bank");

                return;
            }
        }


        public void DoWaveStarted()
        {
            if (!Simulator.DemoMode)
                XACTAudio.PlayCue("SilentCue" /*"sfxNouvelleVague"*/, "Sound Bank");
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
            if (turret.Type == TurretType.Gravitational && !Simulator.DemoMode)
                XACTAudio.PlayCue("SilentCue" /*"sfxTourelleGravitationnelleAchetee"*/, "Sound Bank");
        }


        public void DoTurretSold(Turret turret, SimPlayer player)
        {
            if (turret.Type == TurretType.Gravitational)
                XACTAudio.PlayCue("SilentCue" /*"sfxTourelleGravitationnelleAchetee"*/, "Sound Bank");
            else
                XACTAudio.PlayCue("SilentCue" /*"sfxTourelleVendue"*/, "Sound Bank");
        }


        public void DoTurretFired(Turret turret)
        {
            Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*turret.SfxShooting*/, "Sound Bank");
        }


        public void DoTurretReactivated(Turret turret)
        {
            if (!turret.BackActiveThisTickOverride)
                Core.XACTAudio.XACTAudio.PlayCue("SilentCue" /*"sfxTourelleMiseAJour"*/, "Sound Bank");
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
    }
}

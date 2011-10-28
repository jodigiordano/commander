namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.XACTAudio;


    class AudioPlayer
    {
        private SimPlayer SimPlayer;
        private Cue MovingCue;
        private Cue InstallingTurretCue;
        private Cue OnCelestialBodyCue;
        private Cue OnMothershipCue;
        private Cue OnPinkHoleCue;

        private float ShipSpeed;
        private bool ShipSpeedChanged;


        public AudioPlayer(SimPlayer simPlayer)
        {
            SimPlayer = simPlayer;
            ShipSpeed = 0;
            ShipSpeedChanged = true;
            MovingCue = XACTAudio.GetCue(SimPlayer.InnerPlayer.MovingSound, "Sound Bank");
            InstallingTurretCue = XACTAudio.GetCue("TurretInstalling", "Sound Bank");
        }


        public void TeleportIn()
        {
            Core.XACTAudio.XACTAudio.PlayCue(SimPlayer.InnerPlayer.TeleportInSound, "Sound Bank");
        }


        public void TeleportOut()
        {
            Core.XACTAudio.XACTAudio.PlayCue(SimPlayer.InnerPlayer.TeleportOutSound, "Sound Bank");
        }


        public void Fire()
        {
            Core.XACTAudio.XACTAudio.PlayCue(SimPlayer.InnerPlayer.FiringSound, "Sound Bank");
        }


        public void StartInstallingTurret()
        {
            InstallingTurretCue.PlayOrResume();
        }


        public void StopInstallingTurret()
        {
            InstallingTurretCue.Pause();
        }


        public void StartOnCelestialBody()
        {
            if (OnCelestialBodyCue != null && OnCelestialBodyCue.IsReady)
                OnCelestialBodyCue.Stop();

            OnCelestialBodyCue = XACTAudio.GetCue("PlanetOn", "Sound Bank");
            OnCelestialBodyCue.PlayOrResume();
        }


        public void StopOnCelestialBody()
        {
            if (OnCelestialBodyCue != null)
                OnCelestialBodyCue.Stop();
        }


        public void StartOnMothership()
        {
            if (OnMothershipCue != null && OnMothershipCue.IsReady)
                OnMothershipCue.Stop();

            OnMothershipCue = XACTAudio.GetCue("MothershipOn", "Sound Bank");
            OnMothershipCue.PlayOrResume();
        }


        public void StopOnMothership()
        {
            if (OnMothershipCue != null)
                OnMothershipCue.Stop();
        }


        public void StartOnPinkHole()
        {
            if (OnPinkHoleCue != null && OnPinkHoleCue.IsReady)
                OnPinkHoleCue.Stop();

            OnPinkHoleCue = XACTAudio.GetCue("WarpProximity", "Sound Bank");
            OnPinkHoleCue.PlayOrResume();
        }


        public void StopOnPinkHole()
        {
            if (OnPinkHoleCue != null)
            OnPinkHoleCue.Stop();
        }


        public void PauseLoopingCues()
        {
            MovingCue.MasterPause();
            InstallingTurretCue.MasterPause();

            if (OnCelestialBodyCue != null && OnCelestialBodyCue.IsReady)
                OnCelestialBodyCue.MasterPause();

            if (OnMothershipCue != null && OnMothershipCue.IsReady)
                OnMothershipCue.MasterPause();

            if (OnPinkHoleCue != null && OnPinkHoleCue.IsReady)
                OnPinkHoleCue.MasterPause();
        }


        public void ResumeLoopingCues()
        {
            MovingCue.MasterResume();
            InstallingTurretCue.MasterResume();

            if (OnCelestialBodyCue != null && OnCelestialBodyCue.IsReady)
                OnCelestialBodyCue.MasterResume();

            if (OnMothershipCue != null && OnMothershipCue.IsReady)
                OnMothershipCue.MasterResume();

            if (OnPinkHoleCue != null && OnPinkHoleCue.IsReady)
                OnPinkHoleCue.MasterResume();
        }


        public void StopLoopingCues()
        {
            MovingCue.Stop();
            InstallingTurretCue.Stop();

            if (OnCelestialBodyCue != null)
                OnCelestialBodyCue.Stop();

            if (OnMothershipCue != null)
                OnMothershipCue.Stop();

            if (OnPinkHoleCue != null)
                OnPinkHoleCue.Stop();
        }


        public void Update()
        {
            HandleMovingCue();
        }


        private void HandleMovingCue()
        {
            // Compute the current ship speed
            ComputeShipSpeed();

            // Pause the cue if the spaceship is not moving (and resume it otherwise)
            if (ShipSpeed == 0)
                MovingCue.Pause();
            else
                MovingCue.PlayOrResume();

            // Modify the cue's attribute depending on the speed of the spaceship
            if (ShipSpeedChanged)
                MovingCue.SetVariable("ShipSpeed", ShipSpeed / SimPlayer.MaximumSpeed);
        }


        //private void HandleFiringCue()
        //{
        //    if (InnerPlayer.Firing)
        //        FiringCue.PlayOrResume();
        //    else
        //        FiringCue.Pause();
        //}


        private void ComputeShipSpeed()
        {
            var currentSpeed = SimPlayer.CurrentSpeed;
            var max = Math.Max(Math.Abs(currentSpeed.X), Math.Abs(currentSpeed.Y));

            ShipSpeedChanged = ShipSpeed != max;
            
            ShipSpeed = max;
        }
    }
}

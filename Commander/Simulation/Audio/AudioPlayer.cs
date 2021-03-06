﻿namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.XACTAudio;


    class AudioPlayer
    {
        private SimPlayer InnerPlayer;
        private Cue MovingCue;
        //private Cue FiringCue;

        private float ShipSpeed;
        private bool ShipSpeedChanged;


        public AudioPlayer(SimPlayer innerPlayer)
        {
            InnerPlayer = innerPlayer;
            ShipSpeed = 0;
            ShipSpeedChanged = true;
            MovingCue = XACTAudio.GetCue("ShipMoving", "Sound Bank");
            //FiringCue = XACTAudio.GetCue("ShipFiring", "Sound Bank");
        }


        public void TeleportIn()
        {
            Core.XACTAudio.XACTAudio.PlayCue("ShipTeleportIn", "Sound Bank");
        }


        public void TeleportOut()
        {
            Core.XACTAudio.XACTAudio.PlayCue("ShipTeleportOut", "Sound Bank");
        }


        public void Fire()
        {
            Core.XACTAudio.XACTAudio.PlayCue("ShipFiring", "Sound Bank");
        }


        public void PauseLoopingCues()
        {
            MovingCue.MasterPause();
            //FiringCue.MasterPause();
        }


        public void ResumeLoopingCues()
        {
            MovingCue.MasterResume();
            //FiringCue.MasterResume();
        }


        public void StopLoopingCues()
        {
            MovingCue.Stop();
            //FiringCue.Stop();
        }


        public void Update()
        {
            HandleMovingCue();
            //HandleFiringCue();
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
                MovingCue.SetVariable("ShipSpeed", ShipSpeed / InnerPlayer.MaximumSpeed);
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
            var currentSpeed = InnerPlayer.CurrentSpeed;
            var max = Math.Max(Math.Abs(currentSpeed.X), Math.Abs(currentSpeed.Y));

            ShipSpeedChanged = ShipSpeed != max;
            
            ShipSpeed = max;
        }
    }
}

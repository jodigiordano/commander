namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.XACTAudio;


    class AudioTurretsController
    {
        private List<Turret> Turrets;
        private Dictionary<Turret, Cue> DisabledTurrets;
        private Dictionary<Turret, float> DisabledTurretsLastValues;


        public AudioTurretsController()
        {
            Turrets = new List<Turret>();
            DisabledTurrets = new Dictionary<Turret, Cue>();
            DisabledTurretsLastValues = new Dictionary<Turret, float>();
        }


        public void AddTurretBought(Turret t)
        {
            if (t.Type == TurretType.Gravitational)
            {
                XACTAudio.PlayCue("GravitationalTurretBought", "Sound Bank");
            }

            else
            {
                XACTAudio.PlayCue("TurretBought", "Sound Bank");
                AddDisabledTurret(t);
            }
        }


        public void AddTurretUpgraded(Turret t)
        {
            if (t.Type == TurretType.Gravitational)
            {
                XACTAudio.PlayCue("GravitationalTurretUpgrade", "Sound Bank");
            }

            else
            {
                XACTAudio.PlayCue("TurretBought" /*"TurretUpgradingStart"*/, "Sound Bank");
                AddDisabledTurret(t);
            }
        }


        public void PauseLoopingCues()
        {
            foreach (var t in DisabledTurrets.Values)
                t.MasterPause();
        }


        public void ResumeLoopingCues()
        {
            foreach (var t in DisabledTurrets.Values)
                t.MasterResume();
        }


        public void Update()
        {
            for (int i = Turrets.Count - 1; i >= 0; i--)
            {
                var turret = Turrets[i];
                var currentValue = 1 - turret.DisabledPercentage;
                var lastValue = DisabledTurretsLastValues[turret];
                var cue = DisabledTurrets[turret];

                // upgrade completed
                if (currentValue >= 1)
                {
                    cue.Stop();
                    DisabledTurrets.Remove(turret);
                    DisabledTurretsLastValues.Remove(turret);
                    Turrets.RemoveAt(i);
                    XACTAudio.PlayCue("TurretUpgraded", "Sound Bank");

                    continue;
                }
                
                // diff enough to modify the sound
                else if (currentValue - lastValue > 0.05f)
                {
                    cue.SetVariable("TurretUpgrade", currentValue);
                    DisabledTurretsLastValues[turret] = currentValue;
                }
            }
        }


        private void AddDisabledTurret(Turret t)
        {
            var disabledCue = XACTAudio.GetCue("TurretUpgrading", "Sound Bank");
            disabledCue.SetVariable("TurretUpgrade", 0.1f);
            Turrets.Add(t);
            DisabledTurrets.Add(t, disabledCue);
            DisabledTurretsLastValues.Add(t, 0.1f);
            disabledCue.Play();
        }
    }
}

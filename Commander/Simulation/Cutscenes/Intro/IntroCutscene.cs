namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;


    class IntroCutscene : Cutscene
    {
        public static Dictionary<string, double> Timing;

        private LogoAnimation Logo;
        private DedicationAnimation Dedication;
        private SimulatorAnimation Simulator;
        private CommanderAnimation Commander;
        private LocationAnimation Location;
        private ProtagonistsAnimation Protagonist;
        private MothershipAnimation Mothership;

        private double TimeBeforeMusic = 2000;
        private double Length = 145000;


        public override void Initialize()
        {
            InitializeTiming();

            Logo = new LogoAnimation(Scene);
            Dedication = new DedicationAnimation(Scene);
            Simulator = new SimulatorAnimation(Scene);
            Commander = new CommanderAnimation(Scene);
            Location = new LocationAnimation(Scene);
            Protagonist = new ProtagonistsAnimation(Simulator.Simulator);
            Mothership = new MothershipAnimation(Simulator.Simulator);

            TimeBeforeMusic = 2000;
            Length = 145000;
        }



        public override void Update()
        {
            Length -= Preferences.TargetElapsedTimeMs;
            TimeBeforeMusic -= Preferences.TargetElapsedTimeMs;

            if (TimeBeforeMusic < 0)
            {
                Core.Audio.Audio.PlayMusic("introMusic", false, 0, false);
                TimeBeforeMusic = double.MaxValue;
            }

            if (Simulator != null)
            {
                Simulator.Update();
                Protagonist.Update();
                Mothership.Update();
            }

            Terminated = Length <= 0;
        }


        public override void Draw()
        {
            Logo.Draw();
            Dedication.Draw();
            Simulator.Draw();
            Location.Draw();
            Protagonist.Draw();
            Commander.Draw();
            Mothership.Draw();
        }
        

        public override void Stop()
        {
            Core.Audio.Audio.StopMusic("introMusic", true, 250);
        }


        private void InitializeTiming()
        {
            Timing = new Dictionary<string, double>();

            Timing.Add("LogoIn", 2000);
            Timing.Add("DedicationIn", 10000);
            Timing.Add("SimulatorIn", 18000);
            Timing.Add("LocationIn", 24000);
            Timing.Add("ProtagonistIn", 28000);
            Timing.Add("MothershipArrival", 56000);
            Timing.Add("MothershipLights", 90000);
            Timing.Add("MothershipDestruction", 97000);
            Timing.Add("CommanderIn", 107000);
            Timing.Add("MothershipDeparture", 120000);

            //test
            //Timing.Add("LogoIn", 0);
            //Timing.Add("DedicationIn", 0);
            //Timing.Add("SimulatorIn", 0);
            //Timing.Add("LocationIn", 0);
            //Timing.Add("ProtagonistIn", 0);
            //Timing.Add("MothershipArrival", 115000);
            //Timing.Add("MothershipLights", 15000);
            //Timing.Add("MothershipDestruction", 1125000);
            //Timing.Add("CommanderIn", 112000);
            //Timing.Add("MothershipDeparture", 1135000);
        }
    }
}

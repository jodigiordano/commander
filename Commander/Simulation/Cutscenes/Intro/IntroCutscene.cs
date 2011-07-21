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
        private double Length = 125000;


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
            Length = 125000;
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
                Commander.Update();
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
            Timing.Add("ProtagonistIn", 27000);
            Timing.Add("MothershipArrival", 53000);
            Timing.Add("MothershipLights", 78000);
            Timing.Add("MothershipDestruction", 86000);
            Timing.Add("CommanderIn", 96000);
            Timing.Add("MothershipDeparture", 106000);

            //test
            //Timing.Add("LogoIn", 100000);
            //Timing.Add("DedicationIn", 100000);
            //Timing.Add("SimulatorIn", 100000);
            //Timing.Add("LocationIn", 100000);
            //Timing.Add("ProtagonistIn", 100000);
            //Timing.Add("MothershipArrival", 100000);
            //Timing.Add("MothershipLights", 100000);
            //Timing.Add("MothershipDestruction", 100000);
            //Timing.Add("CommanderIn", 3000);
            //Timing.Add("MothershipDeparture", 100000);
        }
    }
}

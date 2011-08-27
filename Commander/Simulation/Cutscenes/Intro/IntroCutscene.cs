namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;


    class IntroCutscene : Cutscene
    {
        public static Dictionary<string, double> Timing;

        private LogoAnimation Logo;
        private SimulatorAnimation Simulator;
        private CommanderAnimation Commander;
        private LocationAnimation Location;
        private ProtagonistsAnimation Protagonist;
        private MothershipAnimation Mothership;
        private ResistanceAnimation Resistance;

        private double TimeBeforeMusic = 1000;
        private double Length = 82000;


        public override void Initialize()
        {
            InitializeTiming();

            Logo = new LogoAnimation(Scene);
            Simulator = new SimulatorAnimation(Scene);
            Commander = new CommanderAnimation(Scene);
            Location = new LocationAnimation(Scene);
            Mothership = new MothershipAnimation(Simulator.Simulator);
            Protagonist = new ProtagonistsAnimation(Simulator.Simulator, Mothership.Mothership);
            Resistance = new ResistanceAnimation(Simulator.Simulator, Mothership.Mothership);
            Mothership.Battleships = Resistance.Battleships;

            TimeBeforeMusic = 1000;
            Length = 82000;

            Main.MusicController.SwitchTo(MusicContext.Cutscene, "introMusic", false);
        }



        public override void Update()
        {
            Length -= Preferences.TargetElapsedTimeMs;
            TimeBeforeMusic -= Preferences.TargetElapsedTimeMs;

            if (TimeBeforeMusic < 0)
            {
                Main.MusicController.PlayMusic(false, false, false);
                TimeBeforeMusic = double.MaxValue;
            }

            if (Simulator != null)
            {
                Simulator.Update();
                Protagonist.Update();
                Mothership.Update();
                Commander.Update();
                Resistance.Update();
            }

            Terminated = Length <= 0;
        }


        public override void Draw()
        {
            Logo.Draw();
            Simulator.Draw();
            Location.Draw();
            Protagonist.Draw();
            Commander.Draw();
            Mothership.Draw();
            Resistance.Draw();
        }
        

        public override void Stop()
        {
            Main.MusicController.SwitchTo(MusicContext.Other);
        }


        private void InitializeTiming()
        {
            Timing = new Dictionary<string, double>();

            Timing.Add("SimulatorIn", 0);
            Timing.Add("LogoIn", 1000);
            Timing.Add("LocationIn", 3000);
            Timing.Add("ProtagonistIn", 5000);
            Timing.Add("MothershipArrival", 16500);

            Timing.Add("HumanBattleshipsArrival", 32000);
            Timing.Add("HumanBattleshipsFiring", 34000);
            Timing.Add("HumanBattleshipsDestruction", 39000);

            Timing.Add("MothershipLights", 34000);
            Timing.Add("MothershipDestruction", 39000);
            Timing.Add("CommanderIn", 50000);
            Timing.Add("MothershipDeparture", 62000);

            //test
            //Timing.Add("MothershipArrival", 0);
            //Timing.Add("HumanBattleshipsArrival", 6000);
            //Timing.Add("HumanBattleshipsFiring", 9000);
            //Timing.Add("HumanBattleshipsDestruction", 15000);
            //Timing.Add("LogoIn", 100000);
            //Timing.Add("DedicationIn", 100000);
            //Timing.Add("SimulatorIn", 0);
            //Timing.Add("LocationIn", 100000);
            //Timing.Add("ProtagonistIn", 0);
            //Timing.Add("MothershipArrival", 10000);
            //Timing.Add("MothershipLights", 10000);
            //Timing.Add("MothershipDestruction", 10000);
            //Timing.Add("CommanderIn", 100000);
            //Timing.Add("MothershipDeparture", 10000);
        }
    }
}

namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


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

        private double Length = 82000;


        public override void Initialize()
        {
            InitializeTiming();

            Logo = new LogoAnimation(Scene);
            Simulator = new SimulatorAnimation(Scene);
            Commander = new CommanderAnimation(Scene);
            Location = new LocationAnimation(Scene);
            Mothership = new MothershipAnimation(Simulator.Simulator)
            {
                TimeBeforeArrival = IntroCutscene.Timing["MothershipArrival"],
                TimeBeforeLights = IntroCutscene.Timing["MothershipLights"],
                TimeBeforeDestruction = IntroCutscene.Timing["MothershipDestruction"],
                TimeBeforeDeparture = IntroCutscene.Timing["MothershipDeparture"],
                TimeArrival = 5500,
                TimeLights = 10000,
                TimeDeparture = 18000,
                ArrivalZoom = 0.7f,
                DepartureZoom = 1.5f
            };

            Protagonist = new ProtagonistsAnimation(Simulator.Simulator, Mothership.Mothership);
            Resistance = new ResistanceAnimation(Simulator.Simulator, Mothership.Mothership);
            Mothership.Battleships = Resistance.Battleships;
            Mothership.CelestialBodies = Simulator.Simulator.Data.Level.PlanetarySystem;
            Mothership.StartingPosition = new Vector3(Scene.CameraView.Center.X, Scene.CameraView.Top - Mothership.Mothership.Size.Y / 2, 0);
            Mothership.ArrivingPosition = new Vector3(Scene.CameraView.Center.X, Scene.CameraView.Top - Mothership.Mothership.Size.Y / 2 + 360, 0);
            Mothership.DeparturePosition = new Vector3(Scene.CameraView.Center.X, Scene.CameraView.Bottom + Mothership.Mothership.Size.Y, 0);

            Length = 82000;

            Main.MusicController.PlayOrResume("CinematicIntro");
        }



        public override void Update()
        {
            Length -= Preferences.TargetElapsedTimeMs;

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
            Main.MusicController.Stop("CinematicIntro");
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

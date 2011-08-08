namespace EphemereGames.Commander.Cutscenes
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;


    class MothershipAnimation
    {
        enum MothershipState
        {
            None,
            Arrival,
            Lights,
            Destruction,
            Departure
        }


        private Mothership Mothership;
        private Simulator Simulator;
        private MothershipState State;

        private double TimeBeforeArrival;
        private double TimeBeforeLights;
        private double TimeBeforeDestruction;
        private double TimeBeforeDeparture;


        public MothershipAnimation(Simulator simulator)
        {
            Simulator = simulator;

            Mothership = new Mothership(simulator, VisualPriorities.Cutscenes.IntroMothership);
            Mothership.Position = new Vector3(0, -Mothership.Size.Y/2 - 360, 0);

            State = MothershipState.None;
            Simulator.Scene.Camera.Zoom = 1f;

            TimeBeforeArrival = IntroCutscene.Timing["MothershipArrival"];
            TimeBeforeLights = IntroCutscene.Timing["MothershipLights"];
            TimeBeforeDestruction = IntroCutscene.Timing["MothershipDestruction"];
            TimeBeforeDeparture = IntroCutscene.Timing["MothershipDeparture"];
        }


        public void Update()
        {
            TimeBeforeArrival -= Preferences.TargetElapsedTimeMs;
            TimeBeforeLights -= Preferences.TargetElapsedTimeMs;
            TimeBeforeDestruction -= Preferences.TargetElapsedTimeMs;
            TimeBeforeDeparture -= Preferences.TargetElapsedTimeMs;

            switch (State)
            {
                case MothershipState.None:
                    if (TimeBeforeArrival <= 0)
                    {
                        Simulator.Scene.PhysicalEffects.Add(Mothership, Core.Physics.PhysicalEffects.Move(new Vector3(0, -Mothership.Size.Y/2, 0), 0, 5000));
                        Simulator.Scene.VisualEffects.Add(Simulator.Scene.Camera, Core.Visual.VisualEffects.ChangeSize(1f, 0.7f, 0, 5000));
                        
                        foreach (var player in Inputs.Players)
                            Inputs.VibrateController(player, 5000, 0.2f, 0.1f);

                        State = MothershipState.Arrival;
                    }

                    break;
                case MothershipState.Arrival:
                    if (TimeBeforeLights < 0)
                    {
                        Mothership.ActivateDeadlyLights(10000);
                        State = MothershipState.Lights;
                    }

                    break;
                case MothershipState.Lights:
                    if (TimeBeforeDestruction < 0)
                    {
                        Mothership.DestroyEverything();
                        State = MothershipState.Destruction;
                    }

                    break;
                case MothershipState.Destruction:
                    if (TimeBeforeDeparture < 0)
                    {
                        Mothership.DeactivateDeadlyLights(3000);
                        Simulator.Scene.PhysicalEffects.Add(Mothership, Core.Physics.PhysicalEffects.Move(new Vector3(0, Mothership.Position.Y + 5000, 0), 0, 18000));
                        Simulator.Scene.VisualEffects.Add(Simulator.Scene.Camera, Core.Visual.VisualEffects.ChangeSize(0.7f, 1.5f, 0, 17000));

                        foreach (var player in Inputs.Players)
                            Inputs.VibrateController(player, 6000, 0.2f, 0.1f);

                        State = MothershipState.Departure;
                    }
                    break;
                case MothershipState.Departure:
                    break;
            }

            Mothership.Update();
        }


        public void Draw()
        {
            Mothership.Draw();
        }
    }
}

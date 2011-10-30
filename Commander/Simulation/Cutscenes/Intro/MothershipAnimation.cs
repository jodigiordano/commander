namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
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


        public Mothership Mothership;
        public List<HumanBattleship> Battleships;
        public List<CelestialBody> CelestialBodies;

        public double TimeBeforeArrival;
        public double TimeArrival;
        public double TimeBeforeLights;
        public double TimeLights;
        public double TimeBeforeDestruction;
        public double TimeBeforeDeparture;
        public double TimeDeparture;
        public float ArrivalZoom;
        public float DepartureZoom;

        public Vector3 ArrivingPosition;
        public Vector3 DeparturePosition;
        public float CameraZoomOut;

        private Simulator Simulator;
        private MothershipState State;


        public MothershipAnimation(Simulator simulator)
        {
            Simulator = simulator;

            Mothership = new Mothership(Simulator, VisualPriorities.Cutscenes.IntroMothership)
            {
                Direction = new Vector3(0, -1, 0),
                ShowShield = true,
                ShieldImageName = "MothershipHitMask",
                ShieldColor = Colors.Default.AlienBright,
                ShieldAlpha = 100,
                ShieldDistance = 10,
                ShieldSize = 16
            };

            Simulator.AddSpaceship(Mothership);

            State = MothershipState.None;
            Simulator.Scene.Camera.Zoom = 1f;
        }


        public Vector3 StartingPosition
        {
            set { Mothership.Position = value; }
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
                        Simulator.Scene.PhysicalEffects.Add(Mothership, Core.Physics.PhysicalEffects.Move(ArrivingPosition, 0, TimeArrival));
                        Simulator.Scene.VisualEffects.Add(Simulator.Scene.Camera, Core.Visual.VisualEffects.ChangeSize(1f, ArrivalZoom, 0, TimeArrival));

                        foreach (var player in Inputs.Players)
                            Inputs.VibrateControllerLowFrequency(player, TimeArrival, 0.4f);

                        State = MothershipState.Arrival;
                    }

                    break;
                case MothershipState.Arrival:
                    if (TimeBeforeLights < 0)
                    {
                        Mothership.ActivateDeadlyLights(Simulator.Scene, TimeLights);
                        State = MothershipState.Lights;
                    }

                    break;
                case MothershipState.Lights:
                    if (TimeBeforeDestruction < 0)
                    {
                        if (CelestialBodies != null)
                            Mothership.DestroyEverything(Simulator.Scene, CelestialBodies);
                        
                        if (Battleships != null)
                            Mothership.DestroyEverything(Simulator.Scene, Battleships);
                        
                        State = MothershipState.Destruction;
                    }

                    break;
                case MothershipState.Destruction:
                    if (TimeBeforeDeparture < 0)
                    {
                        Mothership.DeactivateDeadlyLights(Simulator.Scene, TimeDeparture / 6);
                        Mothership.CoverInvasionShips(Simulator.Scene, TimeDeparture / 4);
                        Simulator.Scene.PhysicalEffects.Add(Mothership, Core.Physics.PhysicalEffects.Move(DeparturePosition, 0, TimeDeparture));
                        Simulator.Scene.VisualEffects.Add(Simulator.Scene.Camera, Core.Visual.VisualEffects.ChangeSize(ArrivalZoom, DepartureZoom, TimeDeparture / 6, TimeDeparture));

                        foreach (var player in Inputs.Players)
                            Inputs.VibrateControllerLowFrequency(player, TimeDeparture / 3, 0.4f);

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
            Mothership.Draw(Simulator.Scene);
        }
    }
}

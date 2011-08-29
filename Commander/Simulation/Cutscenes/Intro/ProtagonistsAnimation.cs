namespace EphemereGames.Commander.Cutscenes
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ProtagonistsAnimation
    {
        enum ProtagonistState
        {
            None,
            Arrival,
            Abduction,
            GoGetHelp,
            FireAgainAtMothership,
            GoInCenter,
            WarnColonies,
            Teleport
        }

        private SpaceshipCursor Protagonist;
        private SpaceshipCursor Wife;
        private Simulator Simulator;

        private ProtagonistState State;
        private double TimeBeforeArrival;
        private double TimeBeforeAbduction;
        private double TimeBeforeGoGetHelp;
        private double TimeBeforeFireAgainAtMothership;
        private double TimeBeforeGoInCenter;
        private double TimeBeforeWarnColonies;
        private double TimeBeforeTeleport;

        private Path2D PathArrivalProtagonist;
        private Path2D PathArrivalWife;
        private double TimeOnPathProtaAndWife;

        private ManualTextBubble ProtagonistText;
        private Mothership Mothership;


        public ProtagonistsAnimation(Simulator simulator, Mothership mothership)
        {
            Simulator = simulator;
            Mothership = mothership;

            Protagonist = new SpaceshipCursor(simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroProtagonist, new Color(95, 71, 255), "Cursor1", true);
            Wife = new SpaceshipCursor(Simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroWife, new Color(255, 0, 136), "Cursor2", true);

            Protagonist.FadeIn();
            Wife.FadeIn();

            ProtagonistText = new ManualTextBubble(Simulator.Scene, new Text("I must warn the\n\nother colonies!", @"Pixelite") { SizeX = 2 }, Protagonist.Position, VisualPriorities.Cutscenes.IntroProtagonist - 0.00001) { Alpha = 0 };

            State = ProtagonistState.None;

            TimeBeforeArrival = IntroCutscene.Timing["ProtagonistIn"];
            TimeBeforeAbduction = TimeBeforeArrival + 20000;
            TimeBeforeGoGetHelp = TimeBeforeAbduction + 4000;
            TimeBeforeFireAgainAtMothership = TimeBeforeGoGetHelp + 2000;
            TimeBeforeGoInCenter = TimeBeforeFireAgainAtMothership + 18000;
            TimeBeforeWarnColonies = TimeBeforeGoInCenter + 20000;
            TimeBeforeTeleport = TimeBeforeWarnColonies + 10000;


            PathArrivalProtagonist = GetPathArrivalProtagonist();
            PathArrivalWife = GetPathArrivalWife();

            TimeOnPathProtaAndWife = 0;
        }


        public void Update()
        {
            TimeBeforeArrival -= Preferences.TargetElapsedTimeMs;
            TimeBeforeAbduction -= Preferences.TargetElapsedTimeMs;
            TimeBeforeGoGetHelp -= Preferences.TargetElapsedTimeMs;
            TimeBeforeFireAgainAtMothership -= Preferences.TargetElapsedTimeMs;
            TimeBeforeGoInCenter -= Preferences.TargetElapsedTimeMs;
            TimeBeforeWarnColonies -= Preferences.TargetElapsedTimeMs;
            TimeBeforeTeleport -= Preferences.TargetElapsedTimeMs;


            switch (State)
            {
                case ProtagonistState.None:
                    
                    if (TimeBeforeArrival < 0)
                        State = ProtagonistState.Arrival;
                    break;

                case ProtagonistState.Arrival:
                    DoArrival();

                    if (TimeBeforeAbduction < 0)
                    {
                        DoAbduction();
                        State = ProtagonistState.Abduction;
                    }
                    break;

                case ProtagonistState.Abduction:
                    if (TimeBeforeGoGetHelp < 0)
                    {
                        DoGoGetHelp();
                        State = ProtagonistState.GoGetHelp;
                    }
                    break;

                case ProtagonistState.GoGetHelp:
                    if (TimeBeforeFireAgainAtMothership < 0)
                    {
                        DoFireAgainAtMothership();
                        State = ProtagonistState.FireAgainAtMothership;
                    }
                    break;

                case ProtagonistState.FireAgainAtMothership:
                    if (TimeBeforeGoInCenter < 0)
                    {
                        DoGoInCenter();
                        State = ProtagonistState.GoInCenter;
                    }
                    break;

                case ProtagonistState.GoInCenter:
                    if (TimeBeforeWarnColonies < 0)
                    {
                        DoWarnColonies();
                        State = ProtagonistState.WarnColonies;
                    }
                    break;

                case ProtagonistState.WarnColonies:
                    if (TimeBeforeTeleport < 0)
                    {
                        DoTeleport();
                        State = ProtagonistState.Teleport;
                    }
                    break;
            }

            ProtagonistText.Position = Protagonist.Position;
            ProtagonistText.Update();
        }


        private void DoArrival()
        {
            TimeOnPathProtaAndWife += Preferences.TargetElapsedTimeMs;

            Protagonist.Position = new Vector3(PathArrivalProtagonist.GetPosition(TimeOnPathProtaAndWife), 0);
            Wife.Position = new Vector3(PathArrivalWife.GetPosition(TimeOnPathProtaAndWife), 0);

            if (TimeOnPathProtaAndWife > 12000)
            {
                Protagonist.Direction = Mothership.Position - Protagonist.Position;
                Wife.Direction = Mothership.Position - Wife.Position;
            }

            else
            {
                float rotationProtagonist = PathArrivalProtagonist.GetRotation(TimeOnPathProtaAndWife);
                float rotationWife = PathArrivalWife.GetRotation(TimeOnPathProtaAndWife);

                if (!float.IsNaN(rotationWife))
                    Wife.Rotation = rotationWife;

                if (!float.IsNaN(rotationProtagonist))
                    Protagonist.Rotation = rotationProtagonist;


                Protagonist.Direction = Core.Physics.Utilities.AngleToVector(Protagonist.Rotation);
                Wife.Direction = Core.Physics.Utilities.AngleToVector(Wife.Rotation);
            }
        }


        private void DoAbduction()
        {
            Wife.FadeOut();
            Mothership.AbductShip(Simulator.Scene, Wife, Wife.FrontImage.VisualPriority - 0.00001);
            //Simulator.Scene.VisualEffects.Add(Simulator.Scene.Camera, Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, 1.2f, 500, 1000));
            //Simulator.Scene.PhysicalEffects.Add(Simulator.Scene.Camera, Core.Physics.PhysicalEffects.Arrival(Wife.Position, 500, 1000));
        }


        private void DoGoInCenter()
        {
            Protagonist.Direction = Core.Physics.Utilities.AngleToVector(-0.35f);
            Simulator.Scene.PhysicalEffects.Add(Protagonist, Core.Physics.PhysicalEffects.Move(Vector3.Zero, 0, 1000));
            Simulator.Scene.VisualEffects.Add(Protagonist, Core.Visual.VisualEffects.Fade(Protagonist.Alpha, 255, 15000, 1000));
        }


        private void DoFireAgainAtMothership()
        {
            Protagonist.Direction = new Vector3(0, -1, 0);
            Simulator.Scene.PhysicalEffects.Add(Protagonist, Core.Physics.PhysicalEffects.Move(new Vector3(Protagonist.Position.X, 100, 0), 0, 1000));
            Simulator.Scene.VisualEffects.Add(Protagonist, Core.Visual.VisualEffects.Fade(Protagonist.Alpha, 0, 14000, 3000));
        }


        private void DoGoGetHelp()
        {
            Protagonist.Direction = new Vector3(0, 1, 0);
            //Simulator.Scene.VisualEffects.Add(Simulator.Scene.Camera, Core.Visual.VisualEffects.ChangeSize(Simulator.Scene.Camera.Zoom, 0.7f, 1000, 1000));
            //Simulator.Scene.PhysicalEffects.Add(Simulator.Scene.Camera, Core.Physics.PhysicalEffects.Arrival(Vector3.Zero, 1000, 1000));
            Simulator.Scene.PhysicalEffects.Add(Protagonist, Core.Physics.PhysicalEffects.Move(new Vector3(Protagonist.Position.X, 1000, 0), 0, 1000));
        }


        private void DoTeleport()
        {
            Simulator.Scene.VisualEffects.Add(ProtagonistText, Core.Visual.VisualEffects.Fade(ProtagonistText.Alpha, 0, 0, 1000));
            Protagonist.TeleportOut();
            Protagonist.FadeOut();
        }


        private void DoWarnColonies()
        {
            Simulator.Scene.VisualEffects.Add(Protagonist.FrontImage, Core.Visual.VisualEffects.ChangeColor(Color.Red, 0, 5000));
            Simulator.Scene.VisualEffects.Add(ProtagonistText, Core.Visual.VisualEffects.FadeInFrom0(255, 5000, 1000));
        }


        public void Draw()
        {
            Protagonist.Draw();
            Wife.Draw();

            ProtagonistText.Draw();
        }


        private Path2D GetPathArrivalProtagonist()
        {
            var positions = new List<Vector2>();
            var times = new List<double>();

            // arrival
            positions.Add(new Vector2(-300, -450));
            positions.Add(new Vector2(-100, -100));

            times.Add(0);
            times.Add(2);
            
            // messing around
            positions.Add(new Vector2(350, 50));
            positions.Add(new Vector2(0, 300));
            positions.Add(new Vector2(-350, 50));
            positions.Add(new Vector2(350, -200));
            positions.Add(new Vector2(350, 100));

            times.Add(4);
            times.Add(6);
            times.Add(8);
            times.Add(10);
            times.Add(12);

            for (int i = 0; i < times.Count; i++)
                times[i] *= 1000;

            return new Path2D(positions, times);
        }


        private Path2D GetPathArrivalWife()
        {
            var positions = new List<Vector2>();
            var times = new List<double>();

            // arrival
            positions.Add(new Vector2(-200, -450));
            positions.Add(new Vector2(-150, -100));

            times.Add(0);
            times.Add(2);

            // messing around
            positions.Add(new Vector2(0, 0));
            positions.Add(new Vector2(375, 25));
            positions.Add(new Vector2(300, 200));
            positions.Add(new Vector2(-100, 200));
            positions.Add(new Vector2(-175, -75));
            positions.Add(new Vector2(50, -175));
            positions.Add(new Vector2(550, -190));
            positions.Add(new Vector2(200, 150));

            times.Add(4);
            times.Add(5);
            times.Add(6);
            times.Add(7);
            times.Add(8);
            times.Add(9);
            times.Add(10);
            times.Add(12);


            for (int i = 0; i < times.Count; i++)
                times[i] *= 1000;

            return new Path2D(positions, times);
        }


        private List<Path2D> GetPathDepartureBabies()
        {
            throw new System.NotImplementedException();
        }


        private Path2D GetPathDepartureWife()
        {
            throw new System.NotImplementedException();
        }
    }
}

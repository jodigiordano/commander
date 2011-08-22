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
            BabiesGoHome,
            WifeWantsToGoHome,
            WifeGoesHome,
            ProtaWantsToFollowWife
        }

        private SpaceshipCursor Protagonist;
        private SpaceshipCursor Wife;
        private List<SpaceshipCursor> Babies;
        private Simulator Simulator;

        private ProtagonistState State;
        private double TimeBeforeArrival;
        private double TimeBeforeBabiesGoHome;
        private double TimeBeforeWifeWantsToGoHome;
        private double TimeBeforeWifeGoHome;
        private double TimeBeforeProtaWantsToFollowWife;

        private Path2D PathArrivalProtagonist;
        private Path2D PathArrivalWife;
        private Path2D PathDepartureWife;
        private List<Path2D> PathArrivalBabies;
        private List<Path2D> PathDepartureBabies;
        private double TimeOnPathProtaAndWife;
        private double TimeOnPathBabies;

        private ManualTextBubble ProtagonistText;


        public ProtagonistsAnimation(Simulator simulator)
        {
            Simulator = simulator;

            Protagonist = new SpaceshipCursor(simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroProtagonist, new Color(95, 71, 255), "Cursor1", true);
            Wife = new SpaceshipCursor(Simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroWife, new Color(255, 0, 136), "Cursor2", true);
            Babies = new List<SpaceshipCursor>();

            Babies.Add(new SpaceshipCursor(Simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroWife, new Color(255, 227, 48), "Cursor3", true) { Size = 2f });
            Babies.Add(new SpaceshipCursor(Simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroWife, Color.Pink, "Cursor4", true) { Size = 2f });
            Babies.Add(new SpaceshipCursor(Simulator.Scene, new Vector3(-200, -600, 0), 5, VisualPriorities.Cutscenes.IntroWife, new Color(255, 115, 40), "Cursor5", true) { Size = 2f });

            ProtagonistText = new ManualTextBubble(Simulator.Scene, new Text("I must warn the\n\nother colonies!", @"Pixelite") { SizeX = 2 }, Protagonist.Position, VisualPriorities.Cutscenes.IntroProtagonist - 0.00001) { Alpha = 0 };

            State = ProtagonistState.None;

            TimeBeforeArrival = IntroCutscene.Timing["ProtagonistIn"];
            TimeBeforeBabiesGoHome = TimeBeforeArrival + 40000;
            TimeBeforeWifeWantsToGoHome = TimeBeforeArrival + 50000;
            TimeBeforeWifeGoHome = TimeBeforeArrival + 57000;
            TimeBeforeProtaWantsToFollowWife = TimeBeforeArrival + 70000;

            PathArrivalProtagonist = GetPathArrivalProtagonist();
            PathArrivalWife = GetPathArrivalWife();
            PathArrivalBabies = GetPathArrivalBabies();

            TimeOnPathProtaAndWife = 0;
            TimeOnPathBabies = 0;

            Simulator.Scene.VisualEffects.Add(Protagonist.FrontImage, Core.Visual.VisualEffects.ChangeColor(Color.Red, IntroCutscene.Timing["MothershipDeparture"] + 10000, 5000));
            Simulator.Scene.VisualEffects.Add(ProtagonistText, Core.Visual.VisualEffects.FadeInFrom0(255, IntroCutscene.Timing["MothershipDeparture"] + 15000, 1000));
        }


        public void Update()
        {
            TimeBeforeArrival -= Preferences.TargetElapsedTimeMs;
            TimeBeforeBabiesGoHome -= Preferences.TargetElapsedTimeMs;
            TimeBeforeWifeWantsToGoHome -= Preferences.TargetElapsedTimeMs;
            TimeBeforeWifeGoHome -= Preferences.TargetElapsedTimeMs;
            TimeBeforeProtaWantsToFollowWife -= Preferences.TargetElapsedTimeMs;

            if (State != ProtagonistState.None)
            {
                TimeOnPathProtaAndWife += Preferences.TargetElapsedTimeMs;
                TimeOnPathBabies += Preferences.TargetElapsedTimeMs;

                MoveProtagonist();
                MoveWife();
                MoveBabies();

                Protagonist.Direction = Core.Physics.Utilities.AngleToVector(Protagonist.Rotation);
                Wife.Direction = Core.Physics.Utilities.AngleToVector(Wife.Rotation);

                foreach (var b in Babies)
                    b.Direction = Core.Physics.Utilities.AngleToVector(b.Rotation);
            }

            switch (State)
            {
                case ProtagonistState.None:
                    
                    if (TimeBeforeArrival < 0)
                        State = ProtagonistState.Arrival;

                    break;

                case ProtagonistState.Arrival:

                    if (TimeBeforeBabiesGoHome < 0)
                    {
                        foreach (var b in Babies)
                        {
                            b.FadeTime = 3000;
                            b.FadeOut();
                        }

                        State = ProtagonistState.WifeWantsToGoHome;
                    }

                    break;

                case ProtagonistState.WifeWantsToGoHome:

                    if (TimeBeforeWifeGoHome < 0)
                    {
                        Wife.FadeTime = 2000;
                        Wife.FadeOut();

                        State = ProtagonistState.WifeGoesHome;
                    }

                    break;

                case ProtagonistState.WifeGoesHome:

                    if (TimeBeforeProtaWantsToFollowWife < 0)
                        State = ProtagonistState.ProtaWantsToFollowWife;

                    break;
            }

            ProtagonistText.Position = Protagonist.Position;
            ProtagonistText.Update();
        }


        private void MoveBabies()
        {
            for (int i = 0; i < Babies.Count; i++)
            {
                Babies[i].Position = new Vector3(PathArrivalBabies[i].GetPosition(TimeOnPathBabies), 0);

                float rotation = PathArrivalBabies[i].GetRotation(TimeOnPathBabies);

                if (!float.IsNaN(rotation))
                    Babies[i].Rotation = rotation;
            }
        }


        private void MoveProtagonist()
        {
            Protagonist.Position = new Vector3(PathArrivalProtagonist.GetPosition(TimeOnPathProtaAndWife), 0);

            float rotationProtagonist = PathArrivalProtagonist.GetRotation(TimeOnPathProtaAndWife);

            if (!float.IsNaN(rotationProtagonist))
                Protagonist.Rotation = rotationProtagonist;
        }


        private void MoveWife()
        {
            Wife.Position = new Vector3(PathArrivalWife.GetPosition(TimeOnPathProtaAndWife), 0);

            float rotationWife = PathArrivalWife.GetRotation(TimeOnPathProtaAndWife);

            if (!float.IsNaN(rotationWife))
                Wife.Rotation = rotationWife;
        }


        public void Draw()
        {
            Protagonist.Draw();
            Wife.Draw();

            foreach (var b in Babies)
                b.Draw();

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
            positions.Add(new Vector2(550, 100));

            times.Add(4);
            times.Add(6);
            times.Add(8);
            times.Add(10);
            times.Add(12);

            // heart
            positions.Add(new Vector2(0, 400));
            positions.Add(new Vector2(-200, 100));
            positions.Add(new Vector2(-300, -200));
            positions.Add(new Vector2(-100, -300));
            positions.Add(new Vector2(50, -100));
            positions.Add(new Vector2(100, 0));
            positions.Add(new Vector2(-200, 200));
            positions.Add(new Vector2(100, 300));

            times.Add(13);
            times.Add(14);
            times.Add(15);
            times.Add(16);
            times.Add(17);
            times.Add(18);
            times.Add(19);
            times.Add(20);

            // 8
            positions.Add(new Vector2(400, 200));
            positions.Add(new Vector2(500, 100));
            positions.Add(new Vector2(400, 0));
            positions.Add(new Vector2(300, 100));

            times.Add(21);
            times.Add(23);
            times.Add(24);
            times.Add(26);

            // scared
            positions.Add(new Vector2(Main.Random.Next(300, 500), 200));

            var posX = Main.Random.Next(300, 500);

            positions.Add(new Vector2(posX, 100));
            positions.Add(new Vector2(posX, 50));

            times.Add(27);
            times.Add(43);

            // stop wife
            positions.Add(new Vector2(150, 200));
            positions.Add(new Vector2(-250, 200));
            positions.Add(new Vector2(150, 200));
            positions.Add(new Vector2(-250, 200));
            positions.Add(new Vector2(150, 200));
            positions.Add(new Vector2(151, 200));

            times.Add(44);
            times.Add(45);
            times.Add(46);
            times.Add(47);
            times.Add(48);
            times.Add(55);

            // wife home
            positions.Add(new Vector2(-100, 250));

            times.Add(56);

            // goes to wife
            positions.Add(new Vector2(-100, 250));

            times.Add(61);

            positions.Add(new Vector2(0, 0));
            positions.Add(new Vector2(0, 0));

            times.Add(65);
            times.Add(67);

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
            positions.Add(new Vector2(400, 350));

            times.Add(4);
            times.Add(5);
            times.Add(6);
            times.Add(7);
            times.Add(8);
            times.Add(9);
            times.Add(10);
            times.Add(12);

            // heart
            positions.Add(new Vector2(0, 400));
            positions.Add(new Vector2(200, 100));
            positions.Add(new Vector2(300, -200));
            positions.Add(new Vector2(100, -300));
            positions.Add(new Vector2(-50, -100));
            positions.Add(new Vector2(-100, 0));
            positions.Add(new Vector2(200, 200));
            positions.Add(new Vector2(-100, 300));

            times.Add(13);
            times.Add(14);
            times.Add(15);
            times.Add(16);
            times.Add(17);
            times.Add(18);
            times.Add(19);
            times.Add(20);

            // 8
            positions.Add(new Vector2(400, 200));
            positions.Add(new Vector2(500, 100));
            positions.Add(new Vector2(400, 0));
            positions.Add(new Vector2(300, 100));

            times.Add(21);
            times.Add(23);
            times.Add(24);
            times.Add(26);

            // scared
            positions.Add(new Vector2(Main.Random.Next(300, 500), 200));

            var posX = Main.Random.Next(300, 500);

            positions.Add(new Vector2(posX, 100));
            positions.Add(new Vector2(posX, 80));
            positions.Add(new Vector2(posX - 150, 80));
            positions.Add(new Vector2(posX - 200, 80));

            times.Add(27);
            times.Add(41);
            times.Add(42);
            times.Add(43);

            // wants to go home
            positions.Add(new Vector2(200, 200));
            positions.Add(new Vector2(-200, 200));
            positions.Add(new Vector2(200, 200));
            positions.Add(new Vector2(-200, 200));
            positions.Add(new Vector2(-220, 200));

            times.Add(44);
            times.Add(45);
            times.Add(46);
            times.Add(47);
            times.Add(55);

            // go home
            positions.Add(new Vector2(-300, 200));
            positions.Add(new Vector2(-200, 100));
            positions.Add(new Vector2(-200, 100));

            times.Add(58);
            times.Add(59);
            times.Add(60);



            for (int i = 0; i < times.Count; i++)
                times[i] *= 1000;

            return new Path2D(positions, times);
        }


        private List<Path2D> GetPathDepartureBabies()
        {
            throw new System.NotImplementedException();
        }


        private List<Path2D> GetPathArrivalBabies()
        {
            List<Path2D> paths = new List<Path2D>();

            foreach (var b in Babies)
            {
                
                var positions = new List<Vector2>();
                var times = new List<double>();


                // arrival
                positions.Add(new Vector2(-700, -700));
                positions.Add(new Vector2(-700, Main.Random.Next(-400, -300)));
                positions.Add(new Vector2(0, Main.Random.Next(-50, 50)));
                positions.Add(new Vector2(550, Main.Random.Next(150, 250)));

                times.Add(0);
                times.Add(16);
                times.Add(17);
                times.Add(18);


                // messing around
                positions.Add(new Vector2(Main.Random.Next(400, 600), -300));
                positions.Add(new Vector2(Main.Random.Next(200, 400), -250));
                positions.Add(new Vector2(Main.Random.Next(150, 250), -100));

                times.Add(19);
                times.Add(20);
                times.Add(21);

                // circle
                positions.Add(new Vector2(Main.Random.Next(300, 500), 200));
                positions.Add(new Vector2(Main.Random.Next(400, 600), 100));
                positions.Add(new Vector2(Main.Random.Next(300, 500), 0));
                positions.Add(new Vector2(Main.Random.Next(200, 400), 100));


                times.Add(22);
                times.Add(23);
                times.Add(24);
                times.Add(25);

                // scared
                positions.Add(new Vector2(Main.Random.Next(300, 500), 300));

                var posX = Main.Random.Next(300, 500);

                positions.Add(new Vector2(posX, 200));
                positions.Add(new Vector2(posX, 201));

                times.Add(27);
                times.Add(40);

                // departure
                positions.Add(new Vector2(-100, 0));
                positions.Add(new Vector2(-500, 250));
                positions.Add(new Vector2(-500, 350));

                times.Add(41);
                times.Add(42);
                times.Add(43);


                for (int i = 0; i < times.Count; i++)
                    times[i] *= 1000;

                paths.Add(new Path2D(positions, times));
            }

            return paths;
        }


        private Path2D GetPathDepartureWife()
        {
            throw new System.NotImplementedException();
        }
    }
}

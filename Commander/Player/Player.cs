namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class Player : Core.Input.Player
    {
        public Circle Circle;
        public Color Color;
        public string Representation;

        private Vector3 position;


        private static List<Vector3> AvailablesInitalPositions = new List<Vector3>()
        {
            new Vector3(-60, -100, 0),
            new Vector3(-30, -100, 0),
            new Vector3(0, -100, 0),
            new Vector3(30, -100, 0),
            new Vector3(60, -100, 0)
        };


        private static List<Color> AvailablesColors = new List<Color>()
        {
            new Color(255, 0, 136),
            new Color(255, 115, 40),
            new Color(128, 255, 63),
            new Color(95, 71, 255),
            new Color(255, 227, 48)
        };


        private static List<string> AvailablesRepresentations = new List<string>()
        {
            "Vaisseau",
            "Collecteur",
            "Resistance1",
            "Resistance2",
            "Resistance3"
        };


        public Player()
            : base()
        {
            Circle = new Circle(Position, 8);

            int index = 0;

            index = Main.Random.Next(0, AvailablesColors.Count);
            Color = AvailablesColors[index];
            AvailablesColors.RemoveAt(index);

            index = Main.Random.Next(0, AvailablesRepresentations.Count);
            Representation = AvailablesRepresentations[index];
            AvailablesRepresentations.RemoveAt(index);

            index = Main.Random.Next(0, AvailablesInitalPositions.Count);
            Position = AvailablesInitalPositions[index];
            AvailablesInitalPositions.RemoveAt(index);

            KeysToListenTo = KeyboardConfiguration.ToList;
            MouseButtonsToListenTo = MouseConfiguration.ToList;
            GamePadButtonsToListenTo = GamePadConfiguration.ToList;
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                VerifyFrame();
                Circle.Position = position;
            }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            Position += delta * speed;
        }


        private void VerifyFrame()
        {
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.Xbox360DeadZoneV2.X + Circle.Radius, 640 - Preferences.Xbox360DeadZoneV2.X - Circle.Radius);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Circle.Radius, 370 - Preferences.Xbox360DeadZoneV2.Y - Circle.Radius);
        }
    }
}
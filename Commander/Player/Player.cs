﻿namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class Player : Core.Input.Player
    {
        public Circle Circle;
        public Color Color;
        public string ImageName;
        public Vector3 SpawningPosition;

        private Vector3 position;

        private Vector3 InitialPosition;
        private Color InitialColor;
        private string InitialImageName;


        private static List<Vector3> AvailablesSpawingPositions = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(50, 0, 0),
            new Vector3(-50, 0, 0),
            new Vector3(0, 50, 0),
            new Vector3(0, -50, 0)
        };


        private static List<Color> AvailablesColors = new List<Color>()
        {
            new Color(255, 0, 136),
            new Color(255, 115, 40),
            new Color(128, 255, 63),
            new Color(95, 71, 255),
            new Color(255, 227, 48)
        };


        private static List<string> AvailablesImages = new List<string>()
        {
            "Cursor1",
            "Cursor2",
            "Cursor3",
            "Cursor4",
            "Cursor5"
        };


        public Player()
            : base()
        {
            Circle = new Circle(Position, 16);

            KeysToListenTo = KeyboardConfiguration.ToList;
            MouseButtonsToListenTo = MouseConfiguration.ToList;
            GamePadButtonsToListenTo = GamePadConfiguration.ToList;

            InitialColor = Color.Aquamarine;
            InitialPosition = Vector3.Down;
            InitialImageName = null;
        }


        public void ChooseAssets()
        {
            // put the data back in the available lists
            if (InitialColor != Color.Aquamarine)
                AvailablesColors.Add(InitialColor);

            if (InitialPosition != Vector3.Down)
                AvailablesSpawingPositions.Add(InitialPosition);

            if (InitialImageName != null)
                AvailablesImages.Add(InitialImageName);


            // select random data
            int index = 0;

            index = Main.Random.Next(0, AvailablesColors.Count);
            InitialColor = AvailablesColors[index];
            AvailablesColors.RemoveAt(index);

            index = Main.Random.Next(0, AvailablesImages.Count);
            ImageName = AvailablesImages[index];
            AvailablesImages.RemoveAt(index);

            index = Main.Random.Next(0, AvailablesSpawingPositions.Count);
            InitialPosition = AvailablesSpawingPositions[index];
            AvailablesSpawingPositions.RemoveAt(index);


            // keep track of selected data
            Color = InitialColor;
            SpawningPosition = InitialPosition;
            InitialImageName = ImageName;
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
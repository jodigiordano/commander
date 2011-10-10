namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;


    class Player : Core.Input.Player
    {
        public Circle Circle;
        public Color Color;
        public string ImageName;
        public Vector3 SpawningPosition;
        public string FiringSound;
        public string MovingSound;
        public string TeleportInSound;
        public string TeleportOutSound;
        public string ShipBouncingSound;
        public string ShipShieldHitSound;
        public string ShipTurningSound;


        public float MovingSpeed;
        public float RotatingSpeed;

        public InputConfiguration InputConfiguration;
        public KeyboardInputConfiguration KeyboardConfiguration     { get { return InputConfiguration.KeyboardConfiguration; } }
        public MouseInputConfiguration MouseConfiguration           { get { return InputConfiguration.MouseConfiguration; } }
        public GamepadInputConfiguration GamepadConfiguration       { get { return InputConfiguration.GamepadConfiguration; } }

        private Vector3 position;
        private Vector3 InitialPosition;
        private Color InitialColor;
        private string InitialImageName;

        public bool ConnectedThisTick;


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
            Colors.Spaceship.Blue,
            Colors.Spaceship.Green,
            Colors.Spaceship.Orange,
            Colors.Spaceship.Pink,
            Colors.Spaceship.Yellow
        };


        private static List<string> AvailablesImages = new List<string>()
        {
            "Cursor1",
            "Cursor2",
            "Cursor3",
            "Cursor4",
            "Cursor5"
        };


        private static List<string> AvailablesFiringSounds = new List<string>()
        {
            "ShipFiring",
            "Ship2Firing",
            "Ship3Firing",
            "Ship4Firing",
            "Ship4Firing",
        };


        private static List<string> AvailablesMovingSounds = new List<string>()
        {
            "ShipMoving",
            "Ship2Moving",
            "Ship3Moving",
            "Ship4Moving",
            "Ship4Moving",
        };


        private static List<string> AvailablesTeleportInSounds = new List<string>()
        {
            "ShipTeleportIn",
            "Ship2TeleportIn",
            "Ship3TeleportIn",
            "Ship4TeleportIn",
            "Ship4TeleportIn",
        };


        private static List<string> AvailablesTeleportOutSounds = new List<string>()
        {
            "ShipTeleportOut",
            "Ship2TeleportOut",
            "Ship3TeleportOut",
            "Ship4TeleportOut",
            "Ship4TeleportOut",
        };


        private static List<string> AvailablesShipBouncingSounds = new List<string>()
        {
            "ShipBouncing",
            "Ship2Bouncing",
            "Ship3Bouncing",
            "Ship4Bouncing",
            "Ship4Bouncing",
        };


        private static List<string> AvailablesShipShieldHitSounds = new List<string>()
        {
            "ShipShieldHit",
            "Ship2ShieldHit",
            "Ship3ShieldHit",
            "Ship4ShieldHit",
            "Ship4ShieldHit",
        };


        private static List<string> AvailablesShipTurningSounds = new List<string>()
        {
            "SilentCue",
            "SilentCue",
            "SilentCue",
            "SilentCue",
            "SilentCue",
        };


        public Player()
            : base()
        {
            Circle = new Circle(Position, 16);

            if (Preferences.Target == Setting.ArcadeRoyale)
                InputConfiguration = Main.InputsFactory.GetNextArcadeRoyaleConfiguration();
            else
                InputConfiguration = Main.InputsFactory.GetDefaultConfiguration();

            KeysToListenTo = KeyboardConfiguration.ToList;
            MouseButtonsToListenTo = MouseConfiguration.ToList;
            GamePadButtonsToListenTo = GamepadConfiguration.ToList;

            InitialColor = Color.Aquamarine;
            InitialPosition = Vector3.Down;
            InitialImageName = null;

            MovingSpeed = 1.5f;
            RotatingSpeed = 0.1f;
            ConnectedThisTick = false;
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

            FiringSound = AvailablesFiringSounds[index];
            MovingSound = AvailablesMovingSounds[index];
            TeleportInSound = AvailablesTeleportInSounds[index];
            TeleportOutSound = AvailablesTeleportOutSounds[index];
            ShipBouncingSound = AvailablesShipBouncingSounds[index];
            ShipShieldHitSound = AvailablesShipShieldHitSounds[index];
            ShipTurningSound = AvailablesShipTurningSounds[index];

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
                Circle.Position = position;
            }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            Position += delta * speed;
        }
    }
}
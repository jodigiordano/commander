namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;


    class Player
    {
        public SignedInGamer Profile;
        public PlayerIndex Index;
        public bool Connected;
        public bool Master;
        public KeyboardConfiguration KeyboardConfiguration;
        public MouseConfiguration MouseConfiguration;
        public GamePadConfiguration GamePadConfiguration;
        public string Name;

        private Vector3 position;
        private Cercle Cercle;


        public Player(PlayerIndex index)
        {
            Index = index;
            Connected = false;
            Master = false;
            Cercle = new Cercle(Position, 8);
            Name = "Unknown";

            KeyboardConfiguration = new KeyboardConfiguration();
            MouseConfiguration = new MouseConfiguration();
            GamePadConfiguration = new GamePadConfiguration();
        }


        public void Initialize()
        {
            Connected = false;
            Master = false;
            Profile = null;

            EphemereGames.Core.Input.Facade.UpdateInputSource(
                Index,
                MouseConfiguration.ToList,
                GamePadConfiguration.ToList,
                KeyboardConfiguration.ToList);
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                VerifyFrame();
                Cercle.Position = position;
            }
        }


        public void Move(ref Vector3 delta, float speed)
        {
            Position += delta * speed;
        }


        private void VerifyFrame()
        {
            position.X = MathHelper.Clamp(this.Position.X, -640 + Preferences.DeadZoneXbox.X + Cercle.Radius, 640 - Preferences.DeadZoneXbox.X - Cercle.Radius);
            position.Y = MathHelper.Clamp(this.Position.Y, -370 + Preferences.DeadZoneXbox.Y + Cercle.Radius, 370 - Preferences.DeadZoneXbox.Y - Cercle.Radius);
        }
    }
}
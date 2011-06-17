namespace EphemereGames.Core.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;


    public abstract class Player
    {
        public SignedInGamer Profile;
        public PlayerIndex Index;
        public bool Connected;
        public InputType InputType;
        public bool Master;
        public string Name;

        public List<Keys> KeysToListenTo;
        public List<MouseButton> MouseButtonsToListenTo;
        public List<Buttons> GamePadButtonsToListenTo;


        public Player(PlayerIndex index)
        {
            Index = index;
            Connected = false;
            Master = false;
            Name = "Unknown";
            InputType = InputType.None;

            KeysToListenTo = new List<Keys>();
            MouseButtonsToListenTo = new List<MouseButton>();
            GamePadButtonsToListenTo = new List<Buttons>();
        }


        public virtual void Initialize()
        {
            Connected = false;
            Master = false;
            Profile = null;
            Name = "Unknown";
            InputType = InputType.None;
        }


        public void Connect()
        {
            Inputs.ConnectPlayer(this);
        }
    }
}
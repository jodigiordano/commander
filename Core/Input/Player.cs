namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;


    public abstract class Player : IEqualityComparer<Player>
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


        public Player()
        {
            Index = PlayerIndex.One;
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


        public bool Equals(Player x, Player y)
        {
            return x.Index == y.Index && x.InputType == y.InputType;
        }


        public int GetHashCode(Player obj)
        {
            return (int) obj.Index + (int) obj.InputType;
        }
    }
}
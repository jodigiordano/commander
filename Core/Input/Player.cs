namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;

    public enum PlayerState
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected
    }


    public abstract class Player : IEqualityComparer<Player>
    {
        public SignedInGamer Profile;
        public PlayerIndexAlt Index;
        public PlayerState State;
        public InputType InputType;
        public bool Master;
        public string Name;

        public List<Keys> KeysToListenTo;
        public List<MouseButton> MouseButtonsToListenTo;
        public List<Buttons> GamePadButtonsToListenTo;


        public Player()
        {
            Index = PlayerIndexAlt.One;
            State = PlayerState.Disconnected;
            Master = false;
            Name = "Unknown";
            InputType = InputType.None;

            KeysToListenTo = new List<Keys>();
            MouseButtonsToListenTo = new List<MouseButton>();
            GamePadButtonsToListenTo = new List<Buttons>();
        }


        public virtual void Initialize()
        {
            State = PlayerState.Disconnected;
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
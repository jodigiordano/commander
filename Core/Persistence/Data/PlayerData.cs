namespace EphemereGames.Core.Persistence
{
    using System.Xml.Serialization;
    using EasyStorage;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework;


    public abstract class PlayerData : Data
    {
        [XmlIgnore]
        public Player Player;


        // only used for deserialization
        public PlayerData()
        {

        }


        public PlayerData(Player player)
        {
            Player = player;

            Initialize();
        }


        public void Initialize()
        {
            PlayerIndex index =
                Player.InputType == InputType.Gamepad ?
                    (PlayerIndex) Player.Index :
                    PlayerIndex.One;

            SaveDevice = new PlayerSaveDevice(index);
        }
    }
}

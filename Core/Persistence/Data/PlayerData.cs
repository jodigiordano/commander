namespace EphemereGames.Core.Persistence
{
    using System.Xml.Serialization;
    using EasyStorage;
    using EphemereGames.Core.Input;


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
            SaveDevice = new PlayerSaveDevice(Player.Index);
        }
    }
}

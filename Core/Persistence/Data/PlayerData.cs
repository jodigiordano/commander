namespace EphemereGames.Core.Persistence
{
    using EasyStorage;
    using EphemereGames.Core.Input;


    public abstract class PlayerData : Data
    {
        public Player Player;


        public PlayerData(Player player)
        {
            Player = player;
            SaveDevice = new PlayerSaveDevice(Player.Index);
        }
    }
}

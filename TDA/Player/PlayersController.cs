namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;


    class PlayersController
    {
        public Dictionary<PlayerIndex, Player> Players;
        public event NoneHandler PlayerDisconnected;

        private bool MasterAssociated;


        public PlayersController()
        {
            Players = new Dictionary<PlayerIndex, Player>();

            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
                Players.Add(index, new Player(index));

            MasterAssociated = false;
        }


        public void Initialize()
        {
            foreach (var player in Players.Values)
                player.Initialize();

            MasterAssociated = false;

            Core.Input.Facade.PlayerConnection.PlayerConnected += new Core.Input.ConnectHandler(playerConnected);
            Core.Input.Facade.PlayerConnection.PlayerDisconnected += new Core.Input.ConnectHandler(playerDisconnected);
        }


        public void Connect(PlayerIndex index)
        {
            Core.Input.Facade.ConnectPlayer(index);
        }


        public bool IsConnected(PlayerIndex index)
        {
            return Players[index].Connected;
        }


        private void playerConnected(PlayerIndex index, SignedInGamer gamer)
        {
            Player p = Players[index];

            if (!MasterAssociated)
            {
                p.Master = true;
                MasterAssociated = true;
            }

            if (gamer != null)
                p.Profile = gamer;

            p.Connected = true;
        }


        private void playerDisconnected(PlayerIndex index, SignedInGamer gamer)
        {
            Player p = Players[index];

            if (gamer != null)
                p.Profile = null;

            p.Connected = false;
        }


        protected virtual void notifyPlayerDisconnected()
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected();
        }
    }
}

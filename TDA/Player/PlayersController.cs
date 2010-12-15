﻿namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using System.Security.Principal;

    class PlayersController
    {
        public Dictionary<PlayerIndex, Player> Players;
        public event NoneHandler PlayerDisconnected;

        public Player MasterPlayer { get; private set; }


        public PlayersController()
        {
            Players = new Dictionary<PlayerIndex, Player>();

            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
                Players.Add(index, new Player(index));

            MasterPlayer = null;
        }


        public void Initialize()
        {
            foreach (var player in Players.Values)
                player.Initialize();

            MasterPlayer = null;

            Core.Input.Facade.PlayerConnection.PlayerConnected += new Core.Input.ConnectHandler(doPlayerConnected);
            Core.Input.Facade.PlayerConnection.PlayerDisconnected += new Core.Input.ConnectHandler(doPlayerDisconnected);
        }


        public void Connect(PlayerIndex index)
        {
            Core.Input.Facade.ConnectPlayer(index);
        }


        public bool IsConnected(PlayerIndex index)
        {
            return Players[index].Connected;
        }


        private void doPlayerConnected(PlayerIndex index, SignedInGamer gamer)
        {
            Player p = Players[index];

            if (MasterPlayer == null)
            {
                p.Master = true;
                MasterPlayer = p;
            }

            if (gamer != null)
            {
                p.Profile = gamer;
                p.Name = gamer.Gamertag;
            }

            else
            {
                string[] names = WindowsIdentity.GetCurrent().Name.Split('\\');

                if (names.Length >= 2)
                    p.Name = names[1];
            }


            p.Name = p.Name.Trim();
            
            if (p.Name.Length > 5)
                p.Name = p.Name.Substring(0, 5);

            p.Connected = true;
        }


        private void doPlayerDisconnected(PlayerIndex index, SignedInGamer gamer)
        {
            Player p = Players[index];

            if (p.Master)
            {
                p.Master = false;
                MasterPlayer = null;
            }

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
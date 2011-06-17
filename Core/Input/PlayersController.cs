namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;


    class PlayersController
    {
        public List<Player> Players;
        public List<Player> ConnectedPlayers;
        public Player MasterPlayer;
        public bool MouseInUse;


        private List<InputListener> Listeners;


        public PlayersController()
        {
            Players = new List<Player>();
            Listeners = new List<InputListener>();
            ConnectedPlayers = new List<Player>();

            MasterPlayer = null;
            MouseInUse = false;
        }


        public void AddPlayer(Player player)
        {
            Players.Add(player);

            player.Initialize();
        }


        public void Initialize()
        {
            MasterPlayer = null;

            Inputs.PlayerConnection.PlayerConnected += new PlayerHandler(DoPlayerConnected);
            Inputs.PlayerConnection.PlayerDisconnected += new PlayerHandler(DoPlayerDisconnected);
        }


        public void AddListener(InputListener listener)
        {
            Listeners.Add(listener);
        }


        public void RemoveListener(InputListener listener)
        {
            Listeners.Remove(listener);
        }


        private void DoPlayerConnected(Player p)
        {
            if (MasterPlayer == null)
            {
                p.Master = true;
                MasterPlayer = p;
            }

            if (p.Profile != null)
                p.Name = p.Profile.Gamertag;

#if WINDOWS
            else
            {
                string[] names = WindowsIdentity.GetCurrent().Name.Split('\\');

                if (names.Length >= 2)
                    p.Name = names[1];
            }
#endif

            p.Name = p.Name.Trim();
            
            if (p.Name.Length > 5)
                p.Name = p.Name.Substring(0, 5);

            p.Connected = true;

            ConnectedPlayers.Add(p);

            if (p.InputType == InputType.Mouse)
                MouseInUse = true;

            foreach (var l in Listeners)
                l.DoPlayerConnected(p);
        }


        private void DoPlayerDisconnected(Player p)
        {
            if (p.Master)
            {
                p.Master = false;
                MasterPlayer = null;
            }

            p.Profile = null;
            p.Connected = false;
            p.InputType = InputType.None;

            ConnectedPlayers.Remove(p);

            if (p.InputType == InputType.Mouse)
                MouseInUse = false;

            foreach (var l in Listeners)
                l.DoPlayerDisconnected(p);
        }


        internal Player GetPlayer(PlayerIndex index)
        {
            return Players.Find(p => p.Index == index);
        }
    }
}

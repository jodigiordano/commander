namespace EphemereGames.Core.Input
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
#if WINDOWS
    using System.Security.Principal;
#endif


    class PlayersController
    {
        public List<Player> Players;
        public List<Player> ConnectedPlayers;
        public Player MasterPlayer;
        public bool MouseInUse;


        private List<InputListener> Listeners;
        private Stack<PlayerIndex> AvailablesIndexes;


        public PlayersController()
        {
            Players = new List<Player>();
            Listeners = new List<InputListener>();

            AvailablesIndexes = new Stack<PlayerIndex>();
            AvailablesIndexes.Push(PlayerIndex.Four);
            AvailablesIndexes.Push(PlayerIndex.Three);
            AvailablesIndexes.Push(PlayerIndex.Two);
            AvailablesIndexes.Push(PlayerIndex.One);

            ConnectedPlayers = new List<Player>();

            MasterPlayer = null;
            MouseInUse = false;
        }


        public void AddPlayer(Player player)
        {
            Players.Add(player);

            if (AvailablesIndexes.Count != 0)
                player.Index = AvailablesIndexes.Pop();
            else
                player.Index = PlayerIndex.Four;

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


        public void Update()
        {

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

            ConnectedPlayers.Add(p);

            if (p.InputType == InputType.Mouse)
            {
                MouseInUse = true;

                Players[Players.Count - 1].Index = p.Index;
            }

            foreach (var l in Listeners)
                if (l.EnableInputs)
                    l.DoPlayerConnected(p);
        }


        private void DoPlayerDisconnected(Player p)
        {
            ConnectedPlayers.Remove(p);

            if (p.Master)
            {
                p.Master = false;

                if (ConnectedPlayers.Count != 0)
                    ConnectedPlayers[0].Master = true;

                MasterPlayer = (ConnectedPlayers.Count != 0) ? ConnectedPlayers[0] : null;
            }

            p.Profile = null;
            
            if (p.InputType == InputType.Mouse)
                MouseInUse = false;

            p.InputType = InputType.None;

            foreach (var l in Listeners)
                if (l.EnableInputs)
                    l.DoPlayerDisconnected(p);
        }


        internal Player GetPlayer(PlayerIndex index)
        {
            foreach (var p in Players)
                if (p.Index == index)
                    return p;

            return null;
        }
    }
}

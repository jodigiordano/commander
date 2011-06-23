namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorController
    {
        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorGeneralMenuAction, TextMenu> GeneralMenuSubMenus;

        public event EditorPlayerHandler PlayerConnected;
        public event EditorPlayerHandler PlayerDisconnected;
        public event EditorPlayerHandler PlayerChanged;

        private Dictionary<SimPlayer, EditorPlayer> Players;
        private Simulator Simulator;


        public EditorController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<SimPlayer, EditorPlayer>();
        }


        public void Initialize()
        {
            Players.Clear();
        }


        public void DoPlayerConnected(SimPlayer player)
        {
            var editorPlayer = new EditorPlayer(Simulator)
            {
                Circle = player.Circle,
                GeneralMenu = GeneralMenu,
                Color = player.Color
            };


            editorPlayer.Initialize();

            Players.Add(player, editorPlayer);

            NotifyPlayerConnected(editorPlayer);
        }


        public void DoPlayerDisconnected(SimPlayer player)
        {
            var editorPlayer = Players[player];

            Players.Remove(player);

            NotifyPlayerDisconnected(editorPlayer);
        }


        public void Update()
        {
            foreach (var player in Players.Values)
            {
                player.Update();
                NotifyPlayerChanged(player);
            }
        }


        public void Draw()
        {

        }


        public void DoPlayerMoved(SimPlayer p)
        {
            var player = Players[p];

            player.Circle.Position = p.Position;
        }


        private void NotifyPlayerConnected(EditorPlayer player)
        {
            if (PlayerConnected != null)
                PlayerConnected(player);
        }


        private void NotifyPlayerDisconnected(EditorPlayer player)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(player);
        }


        private void NotifyPlayerChanged(EditorPlayer player)
        {
            if (PlayerChanged != null)
                PlayerChanged(player);
        }
    }
}

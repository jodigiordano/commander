namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorGUIController
    {
        public EditorGeneralMenu GeneralMenu;

        private Dictionary<EditorPlayer, EditorGUIPlayer> Players;


        private Simulator Simulator;


        public EditorGUIController(Simulator simulator)
        {
            Simulator = simulator;

            GeneralMenu = new EditorGeneralMenu(simulator, new Vector3(350, 300, 0), Preferences.PrioriteSimulationCorpsCeleste);

            Players = new Dictionary<EditorPlayer, EditorGUIPlayer>();
        }


        public void Initialize()
        {

        }


        public void DoPlayerConnected(EditorPlayer p)
        {
            EditorGUIPlayer player = new EditorGUIPlayer(Simulator, p.Color);

            player.GeneralMenu = GeneralMenu;

            Players.Add(p, player);
        }


        public void DoPlayerDisconnected(EditorPlayer p)
        {
            Players.Remove(p);
        }


        public void DoPlayerChanged(EditorPlayer p)
        {
            var player = Players[p];

            player.SelectedGeneralMenu = p.ActualSelection.GeneralMenu;
        }


        public void Update()
        {

        }


        public void Draw()
        {
            GeneralMenu.Draw();

            foreach (var player in Players.Values)
                player.Draw();
        }
    }
}

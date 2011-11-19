namespace EphemereGames.Commander
{
    class ResetHighscoresProtocol : ServerProtocol
    {
        private int WorldId;


        public ResetHighscoresProtocol(int worldId)
        {
            WorldId = worldId;
        }


        public override void Start()
        {
            AddQuery(ResetHighscoresQuery);
        }


        private string ResetHighscoresQuery
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.ResetHighscoresScript + "?" +
                    Main.PlayersController.MultiverseData.ToUrlArguments +
                    "&world=" + WorldId;
            }
        }
    }
}

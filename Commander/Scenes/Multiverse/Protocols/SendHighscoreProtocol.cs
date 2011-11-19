namespace EphemereGames.Commander
{
    class SendHighscoreProtocol : ServerProtocol
    {
        private int WorldId;
        private int LevelId;
        private int Score;


        public SendHighscoreProtocol(int worldId, int levelId, int score)
        {
            WorldId = worldId;
            LevelId = levelId;
            Score = score;
        }


        public override void Start()
        {
            AddQuery(SubmitHighscoreQuery);
        }


        private string SubmitHighscoreQuery
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.SubmitHighscoreScript + "?" +
                    Main.PlayersController.MultiverseData.ToUrlArguments +
                    "&world=" + WorldId +
                    "&level=" + LevelId +
                    "&score=" + Score;
            }
        }
    }
}

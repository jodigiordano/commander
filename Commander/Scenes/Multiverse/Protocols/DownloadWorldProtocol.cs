namespace EphemereGames.Commander
{
    using System.Text;
    using EphemereGames.Commander.Simulation;

    
    class DownloadWorldProtocol : ServerProtocol
    {
        public int WorldId;

        protected enum SpecificProtocolState
        {
            AskForWorldId,
            AskForLastUpdate,
            DownloadingWorld,
            DownloadingHighscores
        }

        protected SpecificProtocolState SpecificState;

        private bool DownloadOnlyIfNewer;


        public DownloadWorldProtocol(int worldId, bool onlyIfNewer)
        {
            WorldId = worldId;
            DownloadOnlyIfNewer = onlyIfNewer;
        }


        public override void Start()
        {
            GetWorldRemotely();
        }


        protected void GetWorldRemotely()
        {
            if (DownloadOnlyIfNewer)
            {
                SpecificState = SpecificProtocolState.AskForLastUpdate;
                AddQuery(LastUpdateScriptUrl);
            }
            else
            {
                DownloadWorld();
            }
        }


        protected override void DoNextStep(MultiverseMessage previous)
        {
            if (SpecificState == SpecificProtocolState.AskForLastUpdate)
            {
                if (NeedUpdate(previous.Message))
                {
                    DownloadWorld();
                    return;
                }

                else
                {
                    DownloadHighScores();
                    return;
                }
            }


            if (SpecificState == SpecificProtocolState.DownloadingWorld)
            {
                DownloadHighScores();
                return;
            }


            if (SpecificState == SpecificProtocolState.DownloadingHighscores)
            {
                WorldsFactory.CreateWorldHighscoresFromString(WorldId, previous.Message);
                //do not return, just process the answer
            }

            base.DoNextStep(previous);
        }


        private bool NeedUpdate(string toConvert)
        {
            var remoteTimestamp = FormatTimestamp(toConvert);

            return Main.WorldsFactory.GetWorldLastModification(WorldId).CompareTo(remoteTimestamp) < 0;
        }


        private void DownloadWorld()
        {
            SpecificState = SpecificProtocolState.DownloadingWorld;
            Main.WorldsFactory.EmptyWorldDirectory(WorldId);
            DownloadFile(
                WorldsFactory.GetWorldMultiverseRemoteZipFile(WorldId),
                WorldsFactory.GetWorldMultiverseLocalZipFile(WorldId));
        }


        private void DownloadHighScores()
        {
            SpecificState = SpecificProtocolState.DownloadingHighscores;
            AddQuery(HighScoresScriptUrl);
        }


        private string FormatTimestamp(string old)
        {
           StringBuilder sb = new StringBuilder();

           foreach (char c in old)
              if (c != ' ' && c != '-' && c != ':')
                 sb.Append(c);

           return sb.ToString();
        }


        private string LastUpdateScriptUrl
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.LastUpdateScript + "?" +
                    WorldsFactory.WorldToURLArgument(WorldId) + "&" +
                    Main.PlayersController.MultiverseData.ToUrlArguments;
            }
        }


        private string HighScoresScriptUrl
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.HighscoresScript + "?" +
                    WorldsFactory.WorldToURLArgument(WorldId);
            }
        }
    }
}

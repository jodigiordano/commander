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
            DownloadingFile,
            FileDownloaded
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
                SpecificState = SpecificProtocolState.DownloadingFile;
                Main.WorldsFactory.EmptyWorldDirectory(WorldId);
                DownloadFile(
                    WorldsFactory.GetWorldMultiverseRemoteZipFile(WorldId),
                    WorldsFactory.GetWorldMultiverseLocalZipFile(WorldId));
            }
        }


        protected override void DoNextStep(MultiverseMessage previous)
        {
            if (SpecificState == SpecificProtocolState.AskForLastUpdate)
            {
                if (NeedUpdate(previous.Message))
                {
                    SpecificState = SpecificProtocolState.DownloadingFile;
                    Main.WorldsFactory.EmptyWorldDirectory(WorldId);
                    DownloadFile(
                        WorldsFactory.GetWorldMultiverseRemoteZipFile(WorldId),
                        WorldsFactory.GetWorldMultiverseLocalZipFile(WorldId));

                    return;
                }
            }

            base.DoNextStep(previous);
        }


        private bool NeedUpdate(string toConvert)
        {
            var remoteTimestamp = FormatTimestamp(toConvert);

            return Main.WorldsFactory.GetWorldLastModification(WorldId).CompareTo(remoteTimestamp) < 0;
        }


        private string FormatTimestamp(string old)
        {
           StringBuilder sb = new StringBuilder();

           foreach (char c in old)
              if (c != ' ' && c != '-' && c != ':')
                 sb.Append(c);

           return sb.ToString();
        }


        protected override bool ProtocolEnded
        {
            get { return true; }
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
    }
}

namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;


    class DownloadWorldByUsernameProtocol : DownloadWorldProtocol
    {
        private string Username;


        public DownloadWorldByUsernameProtocol(string username, bool onlyIfNewer)
            : base(-1, onlyIfNewer)
        {
            Username = username;
        }


        public override void Start()
        {
            SpecificState = SpecificProtocolState.AskForWorldId;
            AddQuery(UsernameToWorldIdScriptUrl);
        }

        
        protected override void DoNextStep(MultiverseMessage previous)
        {
            if (SpecificState == SpecificProtocolState.AskForWorldId)
            {
                WorldId = int.Parse(previous.Message);

                GetWorldRemotely();
                return;
            }

            base.DoNextStep(previous);
        }


        private string UsernameToWorldIdScriptUrl
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.UsernameToWorldIdScript + "?" +
                    WorldsFactory.WorldUsernameToURLArgument(Username) + "&" +
                    Main.PlayersController.MultiverseData.ToUrlArguments;
            }
        }
    }
}

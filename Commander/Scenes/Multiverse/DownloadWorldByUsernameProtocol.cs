namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Commander.Simulation;


    class DownloadWorldByUsernameProtocol : DownloadWorldProtocol
    {
        private string Username;


        public DownloadWorldByUsernameProtocol(string username, bool onlyIfNewer)
            : base(-1, onlyIfNewer)
        {
            Username = username;

            State = ProtocolState.AskForWorldId;
            Remote.DownloadStringAsync(new Uri(UsernameToWorldIdScriptUrl));
        }


        protected override void Initialize()
        {

        }


        protected override void NextStep(MultiverseMessage previous)
        {
            if (State == ProtocolState.AskForWorldId)
            {
                WorldId = int.Parse(previous.Message);

                GetWorldRemotely();
                return;
            }

            base.NextStep(previous);
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

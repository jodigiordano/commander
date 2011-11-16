namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;


    class RegisterProtocol : ServerProtocol
    {
        private enum SpecificProtocolState
        {
            Registering,
            SavingWorld
        }

        private SpecificProtocolState SpecificState;
        private string Username;
        private string Password;
        private string Email;


        public RegisterProtocol(string username, string password, string email)
        {
            Username = username;
            Password = PlayersController.GetSHA256Hash(password + Preferences.Salt);
            Email = email;
        }


        public override void Start()
        {
            SpecificState = SpecificProtocolState.Registering;
            AddQuery(RegisterQuery);
        }


        protected override void DoNextStep(MultiverseMessage previous)
        {
            if (SpecificState == SpecificProtocolState.Registering)
            {
                var worldId = int.Parse(previous.Message);

                Main.PlayersController.UpdateMultiverse(Username, Password, worldId);
                Main.WorldsFactory.AddMultiverseWorld(Main.WorldsFactory.GetEmptyWorld(worldId, Username));

                SpecificState = SpecificProtocolState.SavingWorld;
                UploadFile(
                    SaveWorldProtocol.GetSaveWorldScriptUrl(worldId),
                    WorldsFactory.GetWorldMultiverseLocalZipFile(worldId));

                return;
            }

            base.DoNextStep(previous);
        }


        protected override bool ProtocolEnded
        {
            get { return true; }
        }


        private string RegisterQuery
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.NewUserScript +
                    "?username=" + Username +
                    "&password=" + Password +
                    "&email=" + Email;
            }
        }
    }
}

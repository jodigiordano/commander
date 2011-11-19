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
        private int WorldId;


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
                WorldId = int.Parse(previous.Message);

                Main.PlayersController.UpdateMultiverse(Username, Password, WorldId);
                Main.WorldsFactory.AddMultiverseWorld(Main.WorldsFactory.GetEmptyWorld(WorldId, Username));

                SpecificState = SpecificProtocolState.SavingWorld;
                UploadFile(
                    SaveWorldProtocol.GetSaveWorldScriptUrl(WorldId),
                    WorldsFactory.GetWorldMultiverseLocalZipFile(WorldId));

                return;
            }

            base.DoNextStep(previous);
        }


        protected override void DoProtocolEndedWithSuccess()
        {
            Main.PlayersController.UpdateMultiverse(Username, Password, WorldId);
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

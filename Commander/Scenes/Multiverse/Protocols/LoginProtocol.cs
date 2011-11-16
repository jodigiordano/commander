namespace EphemereGames.Commander
{


    class LoginProtocol : ServerProtocol
    {
        private string Username;
        private string Password;

        private int WorldId;


        public LoginProtocol(string username, string password)
        {
            Username = username;
            Password = PlayersController.GetSHA256Hash(password + Preferences.Salt);
        }


        public override void Start()
        {
            AddQuery(LoginQuery);
        }


        protected override bool ProtocolEnded
        {
            get { return true; }
        }


        protected override void DoNextStep(MultiverseMessage previous)
        {
            WorldId = int.Parse(previous.Message);

            base.DoNextStep(previous);
        }


        protected override void DoProtocolEndedWithSuccess()
        {
            Main.PlayersController.UpdateMultiverse(Username, Password, WorldId);
        }


        private string LoginQuery
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.LoginScript +
                    "?username=" + Username +
                    "&password=" + Password;
            }
        }
    }
}

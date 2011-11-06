namespace EphemereGames.Commander
{
    using EphemereGames.Core.SimplePersistence;


    public class MultiverseData : SimpleData
    {
        public string Username;
        public string Password;
        public int WorldId;


        public MultiverseData()
        {
            Name = "Multiverse";
            Directory = @"UserData\Multiverse";
            File = "Multiverse.xml";
        }


        public string ToUrlArguments
        {
            get
            {
                return
                    "username=" + Username +
                    "&password=" + Password;
            }
        }


        public bool LoggedIn
        {
            get { return Username != "" && Password != ""; }
        }


        #region Load & Save Handling

        protected override void DoInitialize(object data)
        {
            var d = data as MultiverseData;

            Username = d.Username;
            Password = d.Password;
            WorldId = d.WorldId;
        }


        public override void DoFileNotFound()
        {
            base.DoFileNotFound();

            FirstLoad();
        }


        protected override void DoLoadFailed()
        {
            base.DoLoadFailed();

            FirstLoad();
        }


        private void FirstLoad()
        {
            Username = "";
            Password = "";

            Main.PlayersController.CreateDirectory(Directory);
            Persistence.SaveData(this);
            Loaded = true;
        }

        #endregion
    }
}


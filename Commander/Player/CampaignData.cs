namespace EphemereGames.Commander
{
    using EphemereGames.Core.SimplePersistence;


    public class CampaignData : SimpleData
    {
        public int CurrentWorld;
        public int Version;


        public CampaignData()
        {
            Directory = @"UserData\Campaign";
            File = "Campaign.xml";
        }


        protected override void DoInitialize(object donnee)
        {
            CampaignData d = donnee as CampaignData;

            CurrentWorld = d.CurrentWorld;
            Version = d.Version;
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
            CurrentWorld = 0;
            Version = Preferences.CampaignVersion;

            Main.PlayersController.CreateDirectory(Directory);
            Persistence.SaveData(this);
            Loaded = true;
        }
    }
}

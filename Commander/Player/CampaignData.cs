namespace EphemereGames.Commander
{
    using EphemereGames.Core.SimplePersistence;


    public class CampaignData : SimpleData
    {
        public int CurrentWorld;


        public CampaignData()
        {
            Name = "Campaign";
            Directory = "UserData";
            File = "Campaign.xml";
        }


        //public void ClearAndSave()
        //{
        //    CurrentWorld = 0;

        //    Save();
        //}


        protected override void DoInitialize(object donnee)
        {
            CampaignData d = donnee as CampaignData;

            CurrentWorld = d.CurrentWorld;
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

            Persistence.SaveData(this);
            Loaded = true;
        }
    }
}

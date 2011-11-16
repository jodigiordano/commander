namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;


    class SaveWorldProtocol : ServerProtocol
    {
        private int WorldId;


        public SaveWorldProtocol(int worldId)
        {
            WorldId = worldId;
        }


        public override void Start()
        {
            UploadFile(
                GetSaveWorldScriptUrl(WorldId),
                WorldsFactory.GetWorldMultiverseLocalZipFile(WorldId));
        }


        protected override bool ProtocolEnded
        {
            get { return true; }
        }



        public static string GetSaveWorldScriptUrl(int id)
        {
            return
                Preferences.WebsiteURL +
                Preferences.MultiverseScriptsURL +
                Preferences.SaveWorldScript + "?" +
                Main.PlayersController.MultiverseData.ToUrlArguments + "&" +
                WorldsFactory.WorldToURLArgument(id);
        }
    }
}

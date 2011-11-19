namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;


    class SaveWorldProtocol : ServerProtocol
    {
        private enum SpecificProtocolState
        {
            ResetHighscores,
            SendFile
        }

        private SpecificProtocolState SpecificState;
        private int WorldId;


        public SaveWorldProtocol(int worldId)
        {
            WorldId = worldId;
        }


        public override void Start()
        {
            SpecificState = SpecificProtocolState.ResetHighscores;

            AddQuery(ResetHighscoresQuery);
        }


        protected override void DoNextStep(MultiverseMessage previous)
        {
            if (SpecificState == SpecificProtocolState.ResetHighscores)
            {
                SpecificState = SpecificProtocolState.SendFile;

                UploadFile(
                    GetSaveWorldScriptUrl(WorldId),
                    WorldsFactory.GetWorldMultiverseLocalZipFile(WorldId));

                return;
            }

            base.DoNextStep(previous);
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


        private string ResetHighscoresQuery
        {
            get
            {
                return
                    Preferences.WebsiteURL +
                    Preferences.MultiverseScriptsURL +
                    Preferences.ResetHighscoresScript + "?" +
                    Main.PlayersController.MultiverseData.ToUrlArguments +
                    "&world=" + WorldId;
            }
        }
    }
}

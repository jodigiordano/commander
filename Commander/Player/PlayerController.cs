namespace EphemereGames.Commander
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using EphemereGames.Core.SimplePersistence;


    class PlayersController
    {
        public OptionsData OptionsData;
        public CampaignData CampaignData;
        public MultiverseData MultiverseData;


        public PlayersController()
        {
            OptionsData = new OptionsData();
            CampaignData = new CampaignData();
            MultiverseData = new MultiverseData();

            CreateDirectory(OptionsData.Directory);
            CreateDirectory(CampaignData.Directory);
            CreateDirectory(MultiverseData.Directory);
        }


        public void UpdateProgress(int worldId, int levelId, string player, int score)
        {
            var world = Main.WorldsFactory.Worlds[worldId];

            world.HighScores.Add(levelId, player, score);

            Persistence.SaveData(world.HighScores);
        }


        public void SaveAll()
        {
            SaveOptions();
            SaveCampaign();
        }


        public void LoadMultiverse()
        {
            Persistence.LoadData(MultiverseData);
        }


        public void LoadCampaign()
        {
            Persistence.LoadData(CampaignData);
        }


        public void LoadOptions()
        {
            Persistence.LoadData(OptionsData);
        }


        public void SaveMultiverse()
        {
            Persistence.SaveData(MultiverseData);
        }


        public void SaveCampaign()
        {
            Persistence.SaveData(CampaignData);
        }


        public void SaveOptions()
        {
            Persistence.SaveData(OptionsData);
        }


        public bool IsMultiverseLoaded
        {
            get { return MultiverseData.Loaded; }
        }


        public bool IsCampaignLoaded
        {
            get { return CampaignData.Loaded; }
        }


        public bool IsOptionsLoaded
        {
            get { return OptionsData.Loaded; }
        }


        public void ResetCampaign()
        {
            SetCampaignWorld(0);
            CampaignData.Version = Preferences.CampaignVersion;
            SaveCampaign();

            foreach (var w in Main.WorldsFactory.CampaignWorlds.Values)
            {
                w.HighScores.Clear();
                Persistence.SaveData(w.HighScores);
            }
        }


        public void VerifyCampaign()
        {
            if (CampaignData.Version != Preferences.CampaignVersion)
                ResetCampaign();
        }


        public void DoShowHelpBarChanged(bool value)
        {
            OptionsData.ShowHelpBar = value;
        }


        public void DoFullScreenChanged(bool value)
        {
            OptionsData.FullScreen = value;
        }


        public void DoVolumeMusicChanged(int value)
        {
            OptionsData.MusicVolume = value;
        }


        public void DoVolumeSfxChanged(int value)
        {
            OptionsData.SfxVolume = value;
        }


        public void CreateDirectory(string directory)
        {
            if (Directory.Exists(directory))
                return;

            Directory.CreateDirectory(directory);
        }


        public void ClearDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (var f in Directory.GetFiles(directory))
                File.Delete(f);
        }


        public void SetCampaignWorld(int id)
        {
            if (id != 0 && !Main.WorldsFactory.CampaignWorlds.ContainsKey(id))
                return;

            CampaignData.CurrentWorld = id;
            SaveCampaign();
        }


        public static string GetSHA256Hash(string input)
        {
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(input);
            Byte[] hash = new SHA256CryptoServiceProvider().ComputeHash(data);

            return Convert.ToBase64String(hash);
        }

        
        public void UpdateMultiverse(string username, string password, string world)
        {
            MultiverseData.Username = username;
            MultiverseData.Password = password;
            MultiverseData.WorldId = Int32.Parse(world);
            SaveMultiverse();
        }
    }
}

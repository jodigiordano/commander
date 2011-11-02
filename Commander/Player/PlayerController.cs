﻿namespace EphemereGames.Commander
{
    using System.IO;
    using EphemereGames.Core.SimplePersistence;


    class PlayersController
    {
        public OptionsData OptionsData;
        public CampaignData CampaignData;


        public PlayersController()
        {
            OptionsData = new OptionsData();
            CampaignData = new CampaignData();

            CreateDirectory(OptionsData.Directory);
            CreateDirectory(CampaignData.Directory);
        }


        public void UpdateProgress(int worldId, int levelId, string player, int score)
        {
            var world = Main.LevelsFactory.Worlds[worldId];

            world.HighScores.Add(levelId, player, score);

            Persistence.SaveData(world.HighScores);
        }


        public void SaveAll()
        {
            SaveOptions();
            SaveCampaign();
        }


        public void LoadCampaign()
        {
            Persistence.LoadData(CampaignData);
        }


        public void LoadOptions()
        {
            Persistence.LoadData(OptionsData);
        }


        public void SaveCampaign()
        {
            Persistence.SaveData(CampaignData);
        }


        public void SaveOptions()
        {
            Persistence.SaveData(OptionsData);
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

            foreach (var w in Main.LevelsFactory.CampaignWorlds.Values)
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


        public void SetCampaignWorld(int id)
        {
            if (id != 0 && !Main.LevelsFactory.CampaignWorlds.ContainsKey(id))
                return;

            CampaignData.CurrentWorld = id;
            SaveCampaign();
        }
    }
}

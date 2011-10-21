namespace EphemereGames.Commander
{
    using EphemereGames.Core.Persistence;


    class SaveGameController
    {
        public SharedSaveGame SharedSaveGame;
        public SaveGame PlayerSaveGame;


        public SaveGameController()
        {
            SharedSaveGame = new SharedSaveGame();
        }


        public void Initialize()
        {
            Persistence.AddSharedData(SharedSaveGame);
        }


        public bool IsPlayerSaveGameLoaded
        {
            get { return PlayerSaveGame != null && PlayerSaveGame.IsLoaded; }
        }


        public bool IsSharedSaveGameLoaded
        {
            get { return SharedSaveGame != null && SharedSaveGame.IsLoaded; }
        }


        public Core.Input.Player CurrentPlayer
        {
            get { return PlayerSaveGame == null ? null : PlayerSaveGame.Player; }
        }


        public bool IsLevelUnlocked(int levelId)
        {
            int value;

            return PlayerSaveGame.Progress.TryGetValue(levelId, out value) && value > 0;
        }


        public int GetPlayerHighScore(int levelId)
        {
            if (PlayerSaveGame == null)
                return 0;

            int score = 0;

            PlayerSaveGame.Scores.TryGetValue(levelId, out score);

            return score;
        }


        public void UpdateProgress(string playerName, GameState gameState, int levelId, int score)
        {
            if (PlayerSaveGame != null)
                PlayerSaveGame.UpdateProgress(gameState, levelId, score);
            
            SharedSaveGame.UpdateProgress(playerName, levelId, score);
        }


        public void SaveAll()
        {
            if (PlayerSaveGame != null)
                PlayerSaveGame.Save();
            
            SharedSaveGame.Save();
        }


        public void ReloadPlayerData(Player p)
        {
            PlayerSaveGame = new SaveGame(p);
            Persistence.SetPlayerData(PlayerSaveGame);
            Persistence.LoadData(PlayerSaveGame.Name);
        }


        public void LoadSharedSave()
        {
            Persistence.LoadData(SharedSaveGame.Name);
        }


        public void DoShowHelpBarChanged(bool value)
        {
            SharedSaveGame.ShowHelpBar = value;
        }


        public void DoFullScreenChanged(bool value)
        {
            SharedSaveGame.FullScreen = value;
        }


        public void DoVolumeMusicChanged(int value)
        {
            SharedSaveGame.MusicVolume = value;
        }


        public void DoVolumeSfxChanged(int value)
        {
            SharedSaveGame.SfxVolume = value;
        }
    }
}

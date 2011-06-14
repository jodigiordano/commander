namespace EphemereGames.Core.Persistence
{
    using System;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    
    public static class Persistence
    {
        internal static DataController DataController;
        internal static AssetsController AssetsController;


        public static void Initialize(
            string contentFolderPath,
            string packagesFolderPath,
            GameServiceContainer gsc)
        {
            Preferences.ContentFolderPath = contentFolderPath;
            Preferences.PackagesFolderPath = packagesFolderPath;
            Preferences.GameServiceContainer = gsc;

            DataController = new DataController();
            AssetsController = new AssetsController();
        }


        public static T GetAsset<T>(string name)
        {
            return AssetsController.GetAsset<T>(name);
        }


        public static T GetAssetCopy<T>(string name)
        {
            return AssetsController.GetAssetCopy<T>(name);
        }


        public static void LoadPackage(string package)
        {
            AssetsController.LoadPackage(package);
        }


        public static void UnloadPackage(string package)
        {
            AssetsController.UnloadPackage(package);
        }


        public static bool IsPackageLoaded(string package)
        {
            return AssetsController.IsPackageLoaded(package);
        }


        public static void AddAssetType(IAsset asset)
        {
            AssetsController.AddAssetType(asset);
        }


        public static void AddData(Data data)
        {
            DataController.AddData(data);
        }


        public static void SaveData(string data)
        {
            DataController.Save(data);
        }


        public static void LoadData(string data)
        {
            DataController.Load(data);
        }


        public static bool DataLoaded(string data)
        {
            return DataController.DataLoaded(data);
        }


        public static void Update(GameTime gameTime)
        {
            DataController.Update(gameTime);
        }
    }
}

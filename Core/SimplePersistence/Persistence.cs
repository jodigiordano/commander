namespace EphemereGames.Core.SimplePersistence
{
    using Microsoft.Xna.Framework;

    
    public static class Persistence
    {
        internal static SimpleDataController DataController;
        internal static AssetsController AssetsController;


        public static void Initialize(
            string contentFolderPath,
            string packagesFolderPath,
            GameServiceContainer gsc)
        {
            Preferences.ContentFolderPath = contentFolderPath;
            Preferences.PackagesFolderPath = packagesFolderPath;
            Preferences.GameServiceContainer = gsc;

            DataController = new SimpleDataController();
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


        public static void SaveData(SimpleData data)
        {
            DataController.SaveData(data);
        }


        public static void SaveDataFromString(SimpleData data, string source)
        {
            DataController.SaveDataFromString(data, source);
        }


        public static void SaveDataFromStream(SimpleData data, byte[] source)
        {
            DataController.SaveDataFromStream(data, source);
        }


        public static void LoadData(SimpleData data)
        {
            DataController.LoadData(data);
        }


        public static void LoadDataFromString(SimpleData data, string source)
        {
            DataController.LoadDataFromString(data, source);
        }


        public static void LoadDataFromStream(SimpleData data, byte[] source)
        {
            DataController.LoadDataFromStream(data, source);
        }
    }
}

namespace EphemereGames.Core.Persistance
{
    using System;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    
    public static class Facade
    {
        private static DataController DataController;


        public static void Initialize(
            String contentFolderPath,
            String packagesFolderPath,
            GameServiceContainer gsc,
            ManagedThread threadContent,
            ManagedThread threadData)
        {
            Preferences.ContentFolderPath = contentFolderPath;
            Preferences.PackagesFolderPath = packagesFolderPath;
            Preferences.GameServiceContainer = gsc;
            Preferences.ThreadContent = threadContent;
            Preferences.ThreadData = threadData;

            DataController = new DataController();
        }


        public static T GetAsset<T>(string name)
        {
            return GestionnaireContenu.Instance.recuperer<T>(name);
        }


        public static T GetAssetCopy<T>(string name)
        {
            return GestionnaireContenu.Instance.recupererParCopie<T>(name);
        }


        public static void LoadPackage(string package)
        {
            GestionnaireContenu.Instance.charger(package);
        }


        public static void UnloadPackage(string package)
        {
            GestionnaireContenu.Instance.decharger(package);
        }


        public static bool PackageLoaded(string package)
        {
            return GestionnaireContenu.Instance.estCharge(package);
        }


        public static void AddAsset(IContenu asset)
        {
            GestionnaireContenu.Instance.ajouterTypeAsset(asset);
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


        //public static String getRepertoireContenu()
        //{
        //    return GestionnaireContenu.Instance.RepertoireContenu;
        //}
    }
}

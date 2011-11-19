namespace EphemereGames.Core.SimplePersistence
{
    public static class Persistence
    {
        internal static SimpleDataController DataController;


        public static void Initialize()
        {
            DataController = new SimpleDataController();
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

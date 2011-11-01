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


        public static void LoadData(SimpleData data)
        {
            DataController.LoadData(data);
        }
    }
}

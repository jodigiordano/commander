namespace EphemereGames.Core.SimplePersistence
{
    class SimpleDataController
    {
        public void SaveData(SimpleData data)
        {
            ParallelTasks.Parallel.StartBackground(data.SaveData);
        }


        public void LoadData(SimpleData data)
        {
            ParallelTasks.Parallel.StartBackground(data.LoadData);
        }
    }
}

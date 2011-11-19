namespace EphemereGames.Core.SimplePersistence
{
    class SimpleDataController
    {
        public void SaveData(SimpleData data)
        {
            ParallelTasks.Parallel.StartBackground(data.SaveData);
        }


        public void SaveDataFromString(SimpleData data, string source)
        {
            data.StringSource = source;
            ParallelTasks.Parallel.StartBackground(data.SaveFromString);
        }


        public void SaveDataFromStream(SimpleData data, byte[] source)
        {
            data.ByteSource = source;
            ParallelTasks.Parallel.StartBackground(data.SaveFromStream);
        }


        public void LoadData(SimpleData data)
        {
            ParallelTasks.Parallel.StartBackground(data.LoadData);
        }


        public void LoadDataFromString(SimpleData data, string source)
        {
            data.StringSource = source;
            ParallelTasks.Parallel.StartBackground(data.LoadFromString);
        }


        public void LoadDataFromStream(SimpleData data, byte[] source)
        {
            data.ByteSource = source;
            ParallelTasks.Parallel.StartBackground(data.LoadFromStream);
        }
    }
}

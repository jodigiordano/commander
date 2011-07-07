namespace EphemereGames.Core.Persistence
{
    using System.Collections.Generic;
    using EasyStorage;
    using Microsoft.Xna.Framework;


    class DataController
    {
        private Dictionary<string, Data> Datas;
#if WINDOWS_PHONE
        private IsolatedStorageSaveDevice SaveDevice;
#else
        private SharedSaveDevice SaveDevice;
#endif
        private GameTime GameTime;


        public DataController()
        {
            Datas = new Dictionary<string, Data>();

#if WINDOWS_PHONE
            SaveDevice = new IsolatedStorageSaveDevice();
#else
            SaveDevice = new SharedSaveDevice();
            SaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            SaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            SaveDevice.PromptForDevice();
#endif
        }


        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;
        }


        public void AddData(Data data)
        {
            Datas.Add(data.Name, data);
            data.SaveDevice = SaveDevice;
        }


        public void Save(string dataName)
        {
#if !WINDOWS_PHONE
            SaveDevice.Update(GameTime);
#endif

            ParallelTasks.Parallel.StartBackground(Datas[dataName].Save);
        }


        public void Load(string dataName)
        {
#if !WINDOWS_PHONE
            SaveDevice.Update(GameTime);
#endif

            ParallelTasks.Parallel.StartBackground(Datas[dataName].Load);
        }


        public bool DataLoaded(string dataName)
        {
            return (Datas.ContainsKey(dataName) && Datas[dataName].Loaded);
        }
    }
}

namespace EphemereGames.Core.Persistence
{
    using System.Collections.Generic;
    using EasyStorage;
    using Microsoft.Xna.Framework;
using System;
    

    class DataController
    {
        private Dictionary<string, Data> Datas;
        private SharedSaveDevice SaveDevice;
        private GameTime GameTime;


        public DataController()
        {
            Datas = new Dictionary<string, Data>();

            SaveDevice = new SharedSaveDevice();
            SaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            SaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            SaveDevice.PromptForDevice();
        }


        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;
        }


        public void AddData(Data data)
        {
            this.Datas.Add(data.Name, data);
            data.SaveDevice = SaveDevice;
        }


        public void Save(string dataName)
        {
            SaveDevice.Update(GameTime);

            ParallelTasks.Parallel.StartBackground(Datas[dataName].Save);
        }


        public void Load(string dataName)
        {
            SaveDevice.Update(GameTime);

            ParallelTasks.Parallel.StartBackground(Datas[dataName].Load);
        }


        public bool DataLoaded(string dataName)
        {
            return (Datas.ContainsKey(dataName) && Datas[dataName].Loaded);
        }
    }
}

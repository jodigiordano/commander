namespace EphemereGames.Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EasyStorage;
    using Microsoft.Xna.Framework;
    

    class DataController
    {
        private Dictionary<string, Data> Datas;
        private SharedSaveDevice SaveDevice;


        public DataController()
        {
            Datas = new Dictionary<string, Data>();

            SaveDevice = new SharedSaveDevice();
            //SaveDevice.DeviceSelected += new EventHandler<EventArgs>(DoDeviceSelected);
            SaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            SaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            SaveDevice.PromptForDevice();
        }


        //private void DoDeviceSelected(object sender, EventArgs e)
        //{
        //    foreach (var data in Datas)
        //        if (data.Value.Loaded)
        //            Save(data.Key);
        //        else
        //            Load(data.Key);
        //}


        public void Update(GameTime gameTime)
        {
            if (SaveDevice != null)
                SaveDevice.Update(gameTime);
        }


        public void AddData(Data data)
        {
            this.Datas.Add(data.Name, data);
            data.SaveDevice = SaveDevice;
        }


        public void Save(string dataName)
        {
            Preferences.ThreadData.AddTask(new ThreadTask(Datas[dataName].Save));
        }


        public void Load(string dataName)
        {
            Preferences.ThreadData.AddTask(new ThreadTask(Datas[dataName].Load));
        }


        public bool DataLoaded(string dataName)
        {
            return (Datas.ContainsKey(dataName) && Datas[dataName].Loaded);
        }
    }
}

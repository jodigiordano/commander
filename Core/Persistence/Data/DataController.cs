namespace EphemereGames.Core.Persistence
{
    using System.Collections.Generic;
    using EasyStorage;
    using Microsoft.Xna.Framework;


    class DataController
    {
        private Dictionary<string, SharedData> SharedDatas;
        private PlayerData PlayerData;

#if WINDOWS_PHONE
        private IsolatedStorageSaveDevice SaveDevice;
#else
        private SharedSaveDevice Everyone;

#endif
        private GameTime GameTime;


        public DataController()
        {
            SharedDatas = new Dictionary<string, SharedData>();

#if WINDOWS_PHONE
            SaveDevice = new IsolatedStorageSaveDevice();
#else
            Everyone = new SharedSaveDevice();
            Everyone.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            Everyone.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            Everyone.PromptForDevice();
#endif
        }


        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;
        }


        public void SetPlayerData(PlayerData data)
        {
            PlayerData = data;

            var device = (PlayerSaveDevice) PlayerData.SaveDevice;

            device.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            device.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            device.PromptForDevice(); 
        }


        public void AddSharedData(SharedData data)
        {
            SharedDatas.Add(data.Name, data);
            data.SaveDevice = Everyone;
        }


        public void SaveData(string dataName)
        {
            if (SharedDatas.ContainsKey(dataName))
            {
#if !WINDOWS_PHONE
                Everyone.Update(GameTime);
#endif

                ParallelTasks.Parallel.StartBackground(SharedDatas[dataName].Save);
            }

            else if (PlayerData != null && PlayerData.Name == dataName)
            {
                ParallelTasks.Parallel.StartBackground(PlayerData.Save);
            }

        }


        public void LoadData(string dataName)
        {
            if (SharedDatas.ContainsKey(dataName))
            {
#if !WINDOWS_PHONE
                Everyone.Update(GameTime);
#endif

                ParallelTasks.Parallel.StartBackground(SharedDatas[dataName].Load);
            }

            else if (PlayerData != null && PlayerData.Name == dataName)
            {
                ((PlayerSaveDevice) PlayerData.SaveDevice).Update(GameTime);

                ParallelTasks.Parallel.StartBackground(PlayerData.Load);
            }
        }


        public bool IsDataLoaded(string dataName)
        {
            return (SharedDatas.ContainsKey(dataName) && SharedDatas[dataName].Loaded || (PlayerData != null && PlayerData.Name == dataName && PlayerData.Loaded));
        }
    }
}

namespace Core.Persistance
{
    using System;
    
    public class StorageMessages
    {
        public string Deconnection { get; set; }
        public string DeconnectionShared { get; set; }
        public string DeconnectionForceReselection { get; set; }
        public string DeconnectionForceReselectionShared { get; set; }
        public string DeviceOptionalOptionsA { get; set; }
        public string DeviceOptionalOptionsB { get; set; }
        public string DeviceRequiredOptions { get; set; }
        public string GuideDeviceRequired { get; set; }
        public string GuideDeviceOptional { get; set; }
        public string GuideDeviceOptionalOptionsA { get; set; }
        public string GuideDeviceOptionalOptionsB { get; set; }
        public string GuideDeviceRequiredOptions { get; set; }
        public string GuideCancelForceReselection { get; set; }
        public string GuideCancel { get; set; }

        public StorageMessages()
        {
            Deconnection = DeconnectionShared = "The storage device was disconnected.";
            DeconnectionForceReselection = DeconnectionForceReselectionShared = "Sorry Commander but you must select a storage device to play this game.";
            DeviceOptionalOptionsA = "Select a storage device.";
            DeviceOptionalOptionsB = "Continue without a storage device.";
            DeviceRequiredOptions = "Select one now!";
            GuideDeviceRequired = "Oups!";
            GuideDeviceOptional = "A storage device is required if you want to save your progress.";
            GuideDeviceOptionalOptionsA = "Select one.";
            GuideDeviceOptionalOptionsB = "Continue without one";
            GuideDeviceRequiredOptions = "Oups!";
            GuideCancelForceReselection = "Sorry Commander but you must select a storage device to play this game.";
            GuideCancel = "Do not ignore me, Commander! Select a storage device!";
        }
    }
}

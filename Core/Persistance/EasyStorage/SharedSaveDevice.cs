namespace Core.Persistance
{
    using System;
    using Microsoft.Xna.Framework.GamerServices;    
    /// <summary>
	/// A SaveDevice used for non player-specific saving of data.
	/// </summary>
	sealed class SharedSaveDevice : SaveDevice
	{
		/// <summary>
		/// Creates a new SaveDevice.
		/// </summary>
		/// <param name="storageContainerName">The name to use when opening a StorageContainer.</param>
		public SharedSaveDevice(string storageContainerName) : base(storageContainerName) { }

		/// <summary>
		/// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
		/// method with the desired parameters, using the given callback.
		/// </summary>
		/// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
		protected override void GetStorageDevice(AsyncCallback callback)
		{
			Guide.BeginShowStorageDeviceSelector(callback, null);
		}
	}
}
namespace Core.Persistance
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Storage;
    
    /// <summary>
	/// A base class for an object that maintains a StorageDevice.
	/// </summary>
	/// <remarks>
	/// We implement the three interfaces rather than deriving from GameComponent
	/// just to simplify our constructor and remove the need to pass the Game to
	/// it.
	/// </remarks>
	public abstract class SaveDevice : IGameComponent, IUpdateable, IDisposable, ISaveDevice
	{
		// strings for various message boxes
        private string deconnection;
        private string deconnectionForceReselection;
        private string[] deviceOptionalOptions = { Preferences.StorageMessages.DeviceOptionalOptionsA, "" };
        private static readonly string[] deviceRequiredOptions = new[] { Preferences.StorageMessages.DeviceRequiredOptions };

		// SaveDevice exposes a single file API and does batching under
		// the hood. Each file operation adds increments a reference counter
		// and starts counting down from this value. When that timer hits zero
		// the reference counter is decremented. When that counter reaches zero,
		// the container is closed. This is used as an optimization since disposing
		// a StorageContainer can be upwards of 1/2 of a second.
		private const float containerExpiration = 2f;

		// the list of timers from file operations
		private readonly List<float> timers = new List<float>();

		// the update order for the component and its enabled state
		private int updateOrder;
		private bool enabled = true;

		// the reference counter for the storageContainer
		private int containerRefCount;

		// was the device connected last frame?
		private bool deviceWasConnected;

		// the current state of the SaveDevice
		private SaveDevicePromptState state = SaveDevicePromptState.None;

		// we store the callbacks as fields to reduce run-time allocation and garbage
		private readonly AsyncCallback storageDeviceSelectorCallback;
		private readonly AsyncCallback forcePromptCallback;
		private readonly AsyncCallback reselectPromptCallback;

		// arguments for our two types of events
		private readonly SaveDevicePromptEventArgs promptEventArgs = new SaveDevicePromptEventArgs();
		private readonly SaveDeviceEventArgs eventArgs = new SaveDeviceEventArgs();

		// the actual storage device and container
		private StorageDevice storageDevice;
		private StorageContainer storageContainer;

		/// <summary>
		/// Gets the name of the StorageContainer used by this SaveDevice.
		/// </summary>
		public string StorageContainerName { get; private set; }

		/// <summary>
		/// Gets whether the SaveDevice has a valid StorageDevice.
		/// </summary>
		public bool HasValidStorageDevice
		{
			get { return storageDevice != null && storageDevice.IsConnected; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the SaveDevice is enabled for use.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (EnabledChanged != null)
						EnabledChanged(this, null);
				}
			}
		}

		/// <summary>
		/// Gets or sets the order in which the SaveDevice is updated
		/// in the game. Components with a lower UpdateOrder are updated
		/// first.
		/// </summary>
		public int UpdateOrder
		{
			get { return updateOrder; }
			set
			{
				if (updateOrder != value)
				{
					updateOrder = value;
					if (UpdateOrderChanged != null)
						UpdateOrderChanged(this, null);
				}
			}
		}

        public SaveDevicePromptState Etat
        {
            get { return state; }
        }

		/// <summary>
		/// Get a value indicating whether or not the SaveDevice has been
		/// disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Invoked when a StorageDevice is selected.
		/// </summary>
		public event EventHandler DeviceSelected;

		/// <summary>
		/// Invoked when a StorageDevice selector is canceled.
		/// </summary>
		public event EventHandler<SaveDeviceEventArgs> DeviceSelectorCanceled;

		/// <summary>
		/// Invoked when the user closes a prompt to reselect a StorageDevice.
		/// </summary>
		public event EventHandler<SaveDevicePromptEventArgs> DeviceReselectPromptClosed;

		/// <summary>
		/// Invoked when the StorageDevice is disconnected.
		/// </summary>
		public event EventHandler<SaveDeviceEventArgs> DeviceDisconnected;

		/// <summary>
		/// Fired when the Enabled property has been changed.
		/// </summary>
		public event EventHandler EnabledChanged;

		/// <summary>
		/// Fired when the UpdateOrder property has been changed.
		/// </summary>
		public event EventHandler UpdateOrderChanged;

		/// <summary>
		/// Creates a new SaveDevice.
		/// </summary>
		/// <param name="storageContainerName">The name to use when opening a StorageContainer.</param>
		protected SaveDevice(string storageContainerName)
		{
            if (this is PlayerSaveDevice)
            {
                deconnection = Preferences.StorageMessages.Deconnection;
                deconnectionForceReselection = Preferences.StorageMessages.DeconnectionForceReselection;
                deviceOptionalOptions[1] = Preferences.StorageMessages.DeviceOptionalOptionsA; // on demandera de retourner au SplashScreen
            }

            else if (this is SharedSaveDevice)
            {
                deconnection = Preferences.StorageMessages.DeconnectionShared;
                deconnectionForceReselection = Preferences.StorageMessages.DeconnectionForceReselectionShared;
                deviceOptionalOptions[1] = Preferences.StorageMessages.DeviceOptionalOptionsB; // on demandera de quitter la partie
            }

			storageDeviceSelectorCallback = StorageDeviceSelectorCallback;
			reselectPromptCallback = ReselectPromptCallback;
			forcePromptCallback = ForcePromptCallback;
			StorageContainerName = storageContainerName;
		}

		/// <summary>
		/// Makes sure Dispose is called if the object is collected
		/// to make sure the storageContainer is closed.
		/// </summary>
		~SaveDevice()
		{
			if (!IsDisposed)
				Dispose();
		}

		/// <summary>
		/// Allows the SaveDevice to dispose of itself.
		/// </summary>
		public virtual void Dispose()
		{
			if (IsDisposed)
				return;

			// just dispose the container and disregard the reference counting
			if (storageContainer != null && !storageContainer.IsDisposed)
				storageContainer.Dispose();

			storageDevice = null;
			IsDisposed = true;
		}

		/// <summary>
		/// Allows the SaveDevice to initialize itself.
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// Flags the SaveDevice to prompt for a storage device on the next Update.
		/// </summary>
		public void PromptForDevice()
		{
			// we only let the programmer show the selector if the 
			// SaveDevice isn't busy doing something else.
			if (state == SaveDevicePromptState.None)
				state = SaveDevicePromptState.ShowSelector;
		}

		/// <summary>
		/// Saves a file.
		/// </summary>
		/// <param name="fileName">The file to save.</param>
		/// <param name="saveAction">The save action to perform.</param>
		/// <returns>True if the save completed without errors, false otherwise.</returns>
		public virtual bool Save(string fileName, SaveAction saveAction)
		{
			if (!HasValidStorageDevice) 
				throw new InvalidOperationException("StorageDevice is not valid.");

			// make sure a container is open and resets the expiration timer
			OpenContainer();

			bool success;
			string path = Path.Combine(storageContainer.Path, fileName);
			try
			{
				using (StreamWriter writer = new StreamWriter(path))
					saveAction(writer);
				success = true;
			}
			catch (Exception e)
			{
				success = false;
			}

			// add a timer to expire the container
			timers.Add(containerExpiration);

			return success;
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="fileName">The file to load.</param>
		/// <param name="loadAction">The load action to perform.</param>
		/// <returns>True if the load completed without error, false otherwise.</returns>
		public virtual bool Load(string fileName, LoadAction loadAction)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException("StorageDevice is not valid.");

			// make sure a container is open and resets the expiration timer
			OpenContainer();

			bool success = false;
			string path = Path.Combine(storageContainer.Path, fileName);
			if (File.Exists(path))
			{
				try
				{
					using (StreamReader reader = new StreamReader(path))
						loadAction(reader);
					success = true;
				}
				catch 
				{
					success = false;
				}
			}

			// add a timer to expire the container
			timers.Add(containerExpiration);

			return success;
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="fileName">The file to delete.</param>
		/// <returns>True if the file either doesn't exist or was deleted succesfully, false if the file exists but failed to be deleted.</returns>
		public virtual bool Delete(string fileName)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException("StorageDevice is not valid.");

			// make sure a container is open and resets the expiration timer
			OpenContainer();

			bool success = true;
			string path = Path.Combine(storageContainer.Path, fileName);
			if (File.Exists(path))
			{
				File.Delete(path);
				success = !File.Exists(path);
			}

			// add a timer to expire the container
			timers.Add(containerExpiration);

			return success;
		}

		/// <summary>
		/// Determines if a given file exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		public virtual bool FileExists(string fileName)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException("StorageDevice is not valid.");

			// make sure a container is open and resets the expiration timer
			OpenContainer();

			string path = Path.Combine(storageContainer.Path, fileName);
			bool exists = File.Exists(path);

			// add a timer to expire the container
			timers.Add(containerExpiration);

			return exists;
		}

		/// <summary>
		/// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
		/// method with the desired parameters, using the given callback.
		/// </summary>
		/// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
		protected abstract void GetStorageDevice(AsyncCallback callback);

		/// <summary>
		/// Prepares the SaveDeviceEventArgs to be used for an event.
		/// </summary>
		/// <param name="args">The event arguments to be configured.</param>
		protected virtual void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			args.Response = SaveDeviceEventResponse.Prompt;
			args.PlayerToPrompt = PlayerIndex.One;
		}

		/// <summary>
		/// Allows the component to update itself.
		/// </summary>
		/// <param name="gameTime">The current game timestamp.</param>
		public void Update(GameTime gameTime)
		{
			// make sure gamer services are available for all of our Guide methods we use			
			if (!GamerServicesDispatcher.IsInitialized)
				throw new InvalidOperationException("SaveDevice requries gamer services to operate. Add the GamerServicesComponent to your game.");

			// if we have an open container...
			if (storageContainer != null && !storageContainer.IsDisposed)
			{
				for (int i = timers.Count - 1; i >= 0; i--)
				{
					// update the timer
					float t = timers[i];
					t -= (float)gameTime.ElapsedGameTime.TotalSeconds;

					// if the timer expired, close the container and remove the timer
					if (t <= 0f)
					{
						CloseContainer();
						timers.RemoveAt(i);
					}

					// otherwise update the timer list to have the right value
					else
						timers[i] = t;
				}
			}

			bool deviceIsConnected = storageDevice != null && storageDevice.IsConnected;

			if (!deviceIsConnected && deviceWasConnected)
			{
                // si c'est le device partagé, on vérifie d'abord que le device du joueur ne vient pas de se déconnecter
                //if (this is SharedSaveDevice && Donnees.SaveDevice != null && !Donnees.SaveDevice.HasValidStorageDevice && Donnees.SaveDevice.deviceWasConnected)
                //{
                    // c'est d'abord au save device du joueur de remarquer qu'il est déconnecté (voir Update de Donnees)
                    //return;
                //}

				// if the device was disconnected, fire off the event and handle result
				PrepareEventArgs(eventArgs);

				if (DeviceDisconnected != null)
					DeviceDisconnected(this, eventArgs);

				HandleEventArgResults();
			}
			else if (!deviceIsConnected)
			{
				// we use the try/catch because of the asynchronous nature of the Guide. 
				// the Guide may not be visible when we do our test, but it may open 
				// up after that point and before we've made a call, causing our Guide
				// methods to throw exceptions.
				try
				{
					if (!Guide.IsVisible)
					{
						switch (state)
						{
							// show the normal storage device selector
							case SaveDevicePromptState.ShowSelector:
								state = SaveDevicePromptState.None;
								GetStorageDevice(storageDeviceSelectorCallback);
								break;
								
// since we always have a device on Windows, we #if out this stuff
#if XBOX360
							// the user cancelled the device selector, and we've decided to 
							// see if they want another chance to choose a device
							case SaveDevicePromptState.PromptForCanceled:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									Preferences.StorageMessages.GuideDeviceOptionalOptionsA,
									Preferences.StorageMessages.GuideCancel,
									deviceOptionalOptions,
									0,
									MessageBoxIcon.None,
									reselectPromptCallback,
									null);
								break;

                            // the user cancelled the device selector, and we've decided to 
                            // see if they want another chance to choose a device
                            case SaveDevicePromptState.PromptForCanceled1Choice:
                                // pas de 2ème choix
                                deviceOptionalOptions[1] = Preferences.StorageMessages.GuideDeviceOptionalOptionsB;

                                Guide.BeginShowMessageBox(
                                    eventArgs.PlayerToPrompt,
                                    Preferences.StorageMessages.GuideDeviceOptionalOptionsA,
                                    Preferences.StorageMessages.GuideCancel,
                                    deviceOptionalOptions,
                                    0,
                                    MessageBoxIcon.None,
                                    reselectPromptCallback,
                                    null);
                                break;

							// the user cancelled the device selector, and we've decided to
							// force them to choose again. this message is simply to inform
							// the user of that.	
							case SaveDevicePromptState.ForceCanceledReselection:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									Preferences.StorageMessages.GuideDeviceRequiredOptions,
									Preferences.StorageMessages.GuideCancelForceReselection,
									deviceRequiredOptions,
									0,
									MessageBoxIcon.None,
									forcePromptCallback,
									null);
								break;

							// the device has been disconnected, and we've decided to ask
							// the user if they want to choose a new one
							case SaveDevicePromptState.PromptForDisconnected:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									Preferences.StorageMessages.GuideDeviceOptional,
									deconnection,
									deviceOptionalOptions,
									0,
									MessageBoxIcon.None,
									reselectPromptCallback,
									null);
								break;

                            // the device has been disconnected, and we've decided to ask
                            // the user if they want to choose a new one
                            case SaveDevicePromptState.PromptForDisconnected1Choice:
                                // pas de 2ème choix
                                deviceOptionalOptions[1] = Preferences.StorageMessages.GuideDeviceOptionalOptionsB;

                                Guide.BeginShowMessageBox(
                                    eventArgs.PlayerToPrompt,
                                    Preferences.StorageMessages.GuideDeviceOptional,
                                    deconnection,
                                    deviceOptionalOptions,
                                    0,
                                    MessageBoxIcon.None,
                                    reselectPromptCallback,
                                    null);
                                break;

							// the device has been disconnected, and we've decided to force
							// the user to select a new one. this message is simply to inform
							// the user of that.
							case SaveDevicePromptState.ForceDisconnectedReselection:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									Preferences.StorageMessages.GuideDeviceRequired,
									deconnectionForceReselection,
									deviceRequiredOptions,
									0,
									MessageBoxIcon.None,
									forcePromptCallback,
									null);
								break;
#endif
                            default:
								break;
						}
					}
				}

				// catch this one type of exception just to be safe
				catch (GuideAlreadyVisibleException) { }
			}

			deviceWasConnected = deviceIsConnected;
		}

		/// <summary>
		/// A callback for the BeginStorageDeviceSelectorPrompt.
		/// </summary>
		/// <param name="result">The result of the prompt.</param>
		private void StorageDeviceSelectorCallback(IAsyncResult result)
		{
			//get the storage device
			storageDevice = Guide.EndShowStorageDeviceSelector(result);

			// if we got a valid device, fire off the DeviceSelected event so
			// that the game knows we have a device
			if (storageDevice != null && storageDevice.IsConnected)
			{
				if (DeviceSelected != null)
					DeviceSelected(this, null);
			}

			// if we don't have a valid device
			else
			{
				// prepare our event arguments for use
				PrepareEventArgs(eventArgs);

				// let the game know the device selector was cancelled so it
				// can tell us how to handle this
				if (DeviceSelectorCanceled != null)
					DeviceSelectorCanceled(this, eventArgs);

				// handle the result of the event
				HandleEventArgResults();
			}
		}

		/// <summary>
		/// A callback for either of the message boxes telling users they
		/// have to choose a storage device, either from cancelling the
		/// device selector or disconnecting the device.
		/// </summary>
		/// <param name="result">The result of the prompt.</param>
		private void ForcePromptCallback(IAsyncResult result)
		{
			// just end the message and instruct the SaveDevice to show the selector
			Guide.EndShowMessageBox(result);
			state = SaveDevicePromptState.ShowSelector;
		}

		/// <summary>
		/// A callback for either of the message boxes asking the user
		/// to select a new device, either from cancelling the device
		/// selector or disconnecting the device.
		/// </summary>
		/// <param name="result">The result of the prompt.</param>
		private void ReselectPromptCallback(IAsyncResult result)
		{
			int? choice = Guide.EndShowMessageBox(result);

			// get the device if the user chose the first option
			state = choice.HasValue && choice.Value == 0 ? SaveDevicePromptState.ShowSelector : SaveDevicePromptState.None;

			// fire an event for the game to know the result of the prompt
			promptEventArgs.ShowDeviceSelector = state == SaveDevicePromptState.ShowSelector;
			if (DeviceReselectPromptClosed != null)
				DeviceReselectPromptClosed(this, promptEventArgs);
		}

		/// <summary>
		/// Handles reading from the eventArgs to determine what action to take.
		/// </summary>
		private void HandleEventArgResults()
		{
			// clear the Device reference
			storageDevice = null;

			// determine the next action...
			switch (eventArgs.Response)
			{
				// will have the manager prompt the user with the option of reselecting the storage device
				case SaveDeviceEventResponse.Prompt:
					state = deviceWasConnected
						? SaveDevicePromptState.PromptForDisconnected
						: SaveDevicePromptState.PromptForCanceled;
					break;

                // will have the manager prompt the user with the option of reselecting the storage device
                case SaveDeviceEventResponse.Prompt1Choice:
                    state = deviceWasConnected
                        ? SaveDevicePromptState.PromptForDisconnected1Choice
                        : SaveDevicePromptState.PromptForCanceled1Choice;
                    break;

				// will have the manager prompt the user that the device must be selected
				case SaveDeviceEventResponse.Force:
					state = deviceWasConnected
						? SaveDevicePromptState.ForceDisconnectedReselection
						: SaveDevicePromptState.ForceCanceledReselection;
					break;

				// will have the manager do nothing
				default:
					state = SaveDevicePromptState.None;
					break;
			}
		}

		/// <summary>
		/// Ensures the storageContainer is open and handles incrementing a reference counter.
		/// </summary>
		private void OpenContainer()
		{
			// only open a new container if the current one is closed
			if (storageContainer == null || storageContainer.IsDisposed)
				storageContainer = storageDevice.OpenContainer(StorageContainerName);

			// increment the ref count
			containerRefCount++;
		}

		/// <summary>
		/// Decrements the container reference counter and disposes the storageContainer
		/// when the reference counter reaches zero.
		/// </summary>
		private void CloseContainer()
		{
			//decrement the ref count
			containerRefCount--;

			// if we hit zero, we dispose the container
			if (containerRefCount == 0)
			{
				if (storageContainer != null)
				{
					if (!storageContainer.IsDisposed)
						storageContainer.Dispose();
					storageContainer = null;
				}
			}
		}
	}
}
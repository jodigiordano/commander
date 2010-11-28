namespace Core.Persistance
{
	/// <summary>
	/// Responses for a user canceling the StorageDevice selector
	/// or disconnecting the StorageDevice.
	/// </summary>
	public enum SaveDeviceEventResponse
	{
		/// <summary>
		/// Take no action.
		/// </summary>
		Nothing,
		
		/// <summary>
		/// Affiche une boîte de message pour choisir entre sélectionner
        /// un périphérique ou revenir au SplashScreen/Quitter la partie (suivant le save device)
		/// </summary>
		Prompt,

        /// <summary>
        /// Displays a message box to choose whether to select a new 
        /// device and shows the selector if appropriate.
        /// </summary>
        Prompt1Choice,

		/// <summary>
		/// Displays a message that the user must choose a new device
		/// and shows the device selector.
		/// </summary>
		Force,
	}
}
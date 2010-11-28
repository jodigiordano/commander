namespace Core.Persistance
{
    using System.IO;
    
    /// <summary>
	/// A method for saving a file.
	/// </summary>
	/// <param name="writer">A StreamWriter to use for saving the data.</param>
	/// <returns>True if the saving completed successfully, false otherwise.</returns>
	public delegate void SaveAction(StreamWriter writer);

	/// <summary>
	/// A method for loading a file.
	/// </summary>
	/// <param name="reader">A StreamReader to use for loading the data.</param>
	/// <returns>True if the loading completed successfully, false otherwise.</returns>
	public delegate void LoadAction(StreamReader reader);

	/// <summary>
	/// Defines the interface for an object that can perform file operations.
	/// </summary>
	public interface ISaveDevice
	{
		/// <summary>
		/// Saves a file.
		/// </summary>
		/// <param name="fileName">The file to save.</param>
		/// <param name="saveAction">The save action to perform.</param>
		/// <returns>True if the save completed without errors, false otherwise.</returns>
		bool Save(string fileName, SaveAction saveAction);

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="fileName">The file to load.</param>
		/// <param name="loadAction">The load action to perform.</param>
		/// <returns>True if the load completed without error, false otherwise.</returns>
		bool Load(string fileName, LoadAction loadAction);

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="fileName">The file to delete.</param>
		/// <returns>True if the file either doesn't exist or was deleted succesfully, false if the file exists but failed to be deleted.</returns>
		bool Delete(string fileName);

		/// <summary>
		/// Determines if a given file exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		bool FileExists(string fileName);
	}
}

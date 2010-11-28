namespace Core.Persistance
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;

#if WINDOWS
	/// <summary>
	/// A SaveDevice exclusive to Windows that does not use the XNA Storage APIs or
	/// rely on any of the GamerServices.
	/// </summary>
	public class PCSaveDevice : ISaveDevice
	{
		/// <summary>
		/// Gets or sets the root directory for where file operations
		/// will occur.
		/// </summary>
		/// <remarks>
		/// The default value is a folder in the current user's 
		/// folder under AppData/Roaming/{Game Name}.
		/// </remarks>
		public string RootDirectory { get; set; }

		/// <summary>
		/// Creates a new PCSaveDevice.
		/// </summary>
		/// <param name="gameName">The name of the game in use. Used to initialize the RootDirectory.</param>
		public PCSaveDevice(string gameName)
		{
			RootDirectory = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				gameName);
		}

		/// <summary>
		/// Saves a file.
		/// </summary>
		/// <param name="fileName">The file to save.</param>
		/// <param name="saveAction">The save action to perform.</param>
		/// <returns>True if the save completed without errors, false otherwise.</returns>
		public virtual bool Save(string fileName, SaveAction saveAction)
		{
			if (!Directory.Exists(RootDirectory))
				Directory.CreateDirectory(RootDirectory);

			string path = Path.Combine(RootDirectory, fileName);
			try
			{
				using (StreamWriter writer = new StreamWriter(path))
					saveAction(writer);
				return true;
			}
			catch { }

			return false;
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="fileName">The file to load.</param>
		/// <param name="loadAction">The load action to perform.</param>
		/// <returns>True if the load completed without error, false otherwise.</returns>
		public virtual bool Load(string fileName, LoadAction loadAction)
		{
			if (!Directory.Exists(RootDirectory))
				Directory.CreateDirectory(RootDirectory);

			string path = Path.Combine(RootDirectory, fileName);
			try
			{
				using (StreamReader reader = new StreamReader(path))
					loadAction(reader);
				return true;
			}
			catch { }

			return false;
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="fileName">The file to delete.</param>
		/// <returns>True if the file either doesn't exist or was deleted succesfully, false if the file exists but failed to be deleted.</returns>
		public virtual bool Delete(string fileName)
		{
			if (!Directory.Exists(RootDirectory))
				Directory.CreateDirectory(RootDirectory);

			string path = Path.Combine(RootDirectory, fileName);
			if (File.Exists(path))
			{
				File.Delete(path);
				return !File.Exists(path);
			}
			return true;
		}

		/// <summary>
		/// Determines if a given file exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		public virtual bool FileExists(string fileName)
		{
			if (!Directory.Exists(RootDirectory))
				Directory.CreateDirectory(RootDirectory);

			return File.Exists(Path.Combine(RootDirectory, fileName));
		}
	}
#endif
}

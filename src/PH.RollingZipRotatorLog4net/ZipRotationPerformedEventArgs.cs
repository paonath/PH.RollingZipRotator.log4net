using System;
using System.Collections.Generic;

namespace PH.RollingZipRotatorLog4net
{
	/// <summary>
	/// On Created new Zip rotation file
	/// </summary>
	public class ZipRotationPerformedEventArgs : EventArgs
	{
		/// <summary>
		/// Created zip-file with logs rotated
		/// </summary>
		public string ZipFile { get; set; }

		/// <summary>
		/// Gets or sets the file deleted.
		/// </summary>
		/// <value>
		/// The file deleted.
		/// </value>
		public List<string> FileDeleted { get; set; }

		/// <summary>
		/// Gets or sets the entry added.
		/// </summary>
		/// <value>
		/// The entry added.
		/// </value>
		public List<string> EntryAdded { get; set; }

		/// <summary>
		/// Gets or sets the size of the zip.
		/// </summary>
		/// <value>
		/// The size of the zip.
		/// </value>
		public long ZipSize { get; set; }

		/// <summary>
		/// Gets the UTC fired.
		/// </summary>
		/// <value>
		/// The UTC fired.
		/// </value>
		public DateTime UtcFired { get; private set; }

		internal ZipRotationPerformedEventArgs()
		{
			FileDeleted = new List<string>();
			EntryAdded  = new List<string>();
			UtcFired    = DateTime.UtcNow;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ZipRotationPerformedEventArgs"/> class.
		/// </summary>
		/// <param name="zipFile">The zip file.</param>
		/// <param name="added">The added.</param>
		/// <param name="deleted">The deleted.</param>
		/// <param name="zipSize">Size of the zip.</param>
		public ZipRotationPerformedEventArgs(string zipFile, List<string> added, List<string> deleted, long zipSize)
			: this()
		{
			ZipFile     = zipFile;
			EntryAdded  = added;
			FileDeleted = deleted;
			ZipSize     = zipSize;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"ZipFile '{ZipFile}' - Size {ZipSize} - Added {EntryAdded.Count} - Deleted {EntryAdded.Count}";
		}
	}
}
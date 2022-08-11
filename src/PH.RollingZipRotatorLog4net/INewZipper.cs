using System;
using System.Collections.Generic;
using System.IO;


namespace PH.RollingZipRotatorLog4net
{
	/// <summary>
	/// 
	/// </summary>
	public interface INewZipper
	{
		/// <summary>
		/// Adds the entries.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <param name="zipArchiveName">Name of the zip archive.</param>
		/// <param name="level">The level.</param>
		void AddEntries(Dictionary<string, FileInfo> files, string zipArchiveName, Ionic.Zlib.CompressionLevel level);

		/// <summary>
		/// Adds the entry to zip.
		/// </summary>
		/// <param name="f">The f.</param>
		/// <param name="entryName">Name of the entry.</param>
		/// <param name="zipArchiveName">Name of the zip archive.</param>
		/// <param name="level">The level.</param>
		void AddEntryToZip(FileInfo f, string entryName, string zipArchiveName, Ionic.Zlib.CompressionLevel level);

		/// <summary>
		/// Adds the files and delete from disk.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <param name="zipArchiveName">Name of the zip archive.</param>
		/// <param name="level">The level.</param>
		void AddFilesAndDeleteFromDisk(IEnumerable<FileInfo> files, string zipArchiveName,
		                               Ionic.Zlib.CompressionLevel level);

		/// <summary>
		/// Occurs when [log rotated].
		/// </summary>
		event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using JetBrains.Annotations;

namespace PH.RollingZipRotatorLog4net
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="PH.RollingZipRotatorLog4net.INewZipper" />
	public class NewZipper : INewZipper
	{
		private readonly object _zipLock = new object();


		/// <summary>
		/// Adds the entries.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <param name="zipArchiveName">Name of the zip archive.</param>
		/// <param name="level">The level.</param>
		public void AddEntries(Dictionary<string, FileInfo> files, string zipArchiveName, Ionic.Zlib.CompressionLevel level)
		{
			try
			{
				var entryAdded   = new List<string>();
				var entryDeleted = new List<string>();

				if (files.Count > 0)
				{
					lock (_zipLock)
					{
						var filesToDelete = new List<string>();
						//

						using (var izip = new ZipFile(zipArchiveName))
						{
							izip.CompressionLevel = level;
							foreach (var keyValuePair in files)
							{
								try
								{
									if (keyValuePair.Value.Exists)
									{
										var e = izip.AddFile(keyValuePair.Value.FullName);

										e.FileName = keyValuePair.Key;
										entryAdded.Add(e.FileName);

										filesToDelete.Add(keyValuePair.Value.FullName);
									}
								}
								catch
								{
									//
								}
							}

							izip.Save();
						}

						foreach (var fileToDelete in filesToDelete)
						{
							try
							{
								File.Delete(fileToDelete);
								entryDeleted.Add(fileToDelete);
							}
							catch
							{
							}
						}

						var ziFileInfo = new FileInfo(zipArchiveName);

						OnLogRotated(new ZipRotationPerformedEventArgs(zipArchiveName, entryAdded, entryDeleted,
						                                               ziFileInfo.Length));
					}
				}
			}
			catch /*(Exception exception)*/
			{
				//
			}
		}

		/// <summary>
		/// Adds the entry to zip.
		/// </summary>
		/// <param name="f">The f.</param>
		/// <param name="entryName">Name of the entry.</param>
		/// <param name="zipArchiveName">Name of the zip archive.</param>
		/// <param name="level">The level.</param>
		public void AddEntryToZip(FileInfo f, [NotNull] string entryName, string zipArchiveName,
		                          Ionic.Zlib.CompressionLevel level)
		{
			var d = new Dictionary<string, FileInfo>() { { entryName, f } };
			AddEntries(d, zipArchiveName, level);
		}

		/// <summary>
		/// Adds the files and delete from disk.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <param name="zipArchiveName">Name of the zip archive.</param>
		/// <param name="level">The level.</param>
		public void AddFilesAndDeleteFromDisk([NotNull] IEnumerable<FileInfo> files, string zipArchiveName,
		                                      Ionic.Zlib.CompressionLevel level)
		{
			var fileInfos = files as FileInfo[] ?? files.ToArray();
			if (fileInfos.Any())
			{
				var filePerDates = fileInfos.OrderBy(x => x.LastWriteTimeUtc).ToList();
				var d            = new Dictionary<string, FileInfo>();
				foreach (var filePerDate in filePerDates)
				{
					d.Add(filePerDate.Name, filePerDate);
				}

				AddEntries(d, zipArchiveName, level);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:LogRotated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="ZipRotationPerformedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnLogRotated(ZipRotationPerformedEventArgs e)
		{
			LogRotated?.Invoke(this, e);
		}

		/// <summary>
		/// Occurs when [log rotated].
		/// </summary>
		public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
	}
}
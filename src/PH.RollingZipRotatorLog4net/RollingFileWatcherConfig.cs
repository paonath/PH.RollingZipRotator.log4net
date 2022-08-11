using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace PH.RollingZipRotatorLog4net
{
	/// <summary>
	/// 
	/// </summary>
	public class RollingFileWatcherConfig
	{
		/// <summary>
		/// Gets or sets a value indicating whether [debug enabled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [debug enabled]; otherwise, <c>false</c>.
		/// </value>
		[XmlAttribute("debug")]
		public bool DebugEnabled { get; set; } = false;

		/// <summary>
		/// Gets or sets the override directory path for zip.
		/// </summary>
		/// <value>
		/// The override directory path for zip.
		/// </value>
		[XmlAttribute("overrideDirectoryPathForZip")]
		public string OverrideDirectoryPathForZip { get; set; } = "";

		/// <summary>
		/// Gets or sets the appender names to exclude.
		/// </summary>
		/// <value>
		/// The appender names to exclude.
		/// </value>
		[XmlElement("appenderNamesToExclude")]
		public string[] AppenderNamesToExclude { get; set; } = null;


		/// <summary>
		/// Saves to file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <exception cref="System.ArgumentException">File '{filePath}' already exists!</exception>
		public void SaveToFile(string filePath)
		{
			var fi = new FileInfo(filePath);
			if (fi.Exists)
			{
				throw new ArgumentException($"File '{filePath}' already exists!", filePath);
			}

			var serializer = new XmlSerializer(typeof(RollingFileWatcherConfig));
			var settings   = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			using (var f = new FileStream(filePath, FileMode.CreateNew))
			{
				serializer.Serialize(f, this, ns);
			}
		}

		private static RollingFileWatcherConfig ReadFromFileInfo(FileInfo fileConfig)
		{
			if (!fileConfig.Exists)
			{
				throw new ArgumentException($"File '{fileConfig.FullName}' does not exists", nameof(fileConfig));
			}

			var serializer = new XmlSerializer(typeof(RollingFileWatcherConfig));
			var settings   = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
			var ns         = new XmlSerializerNamespaces();
			ns.Add("", "");

			using (var f = new FileStream(fileConfig.FullName, FileMode.Open))
			{
				var cfg = (RollingFileWatcherConfig)serializer.Deserialize(f);
				return cfg;
			}
		}

		/// <summary>
		/// Reads from file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		public static RollingFileWatcherConfig ReadFromFile(string filePath)
		{
			var fi = new FileInfo(filePath);
			return ReadFromFileInfo(fi);
		}
	}
}
using System;

namespace PH.RollingZipRotatorLog4net
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public interface IRollingFileWatcher : IDisposable
	{
		/// <summary>
		/// Gets the name of the directory.
		/// </summary>
		/// <value>
		/// The name of the directory.
		/// </value>
		string DirectoryName { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IRollingFileWatcher"/> is disposed.
		/// </summary>
		/// <value>
		///   <c>true</c> if disposed; otherwise, <c>false</c>.
		/// </value>
		bool Disposed { get; }

		/// <summary>
		/// Watches this instance.
		/// </summary>
		/// <returns></returns>
		IRollingFileWatcher Watch();

		/// <summary>
		/// Occurs when [log rotated].
		/// </summary>
		event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
	}
}
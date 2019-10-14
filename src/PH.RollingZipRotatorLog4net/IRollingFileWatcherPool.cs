using System;

namespace PH.RollingZipRotatorLog4net
{

    public interface IRollingFileWatcherPoolCommon : IDisposable
    {
        /// <summary>
        /// If disposed
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IRollingFileWatcherPoolCommon"/> is debug.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        bool Debug { get; }

        /// <summary>
        /// if watching for rotation:
        /// remeber to start watching using StartWatch method.
        /// </summary>
        bool Watching { get; }


        /// <summary>Occurs when log rotated.</summary>
        event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }

    public interface IConfiguredRollingFileWatcherPool : IRollingFileWatcherPoolCommon
    {
        /// <summary>Starts the watch.</summary>
        /// <returns></returns>
        IRollingFileWatcherPool StartWatch();
    }

    /// <summary>
    /// A pool of filesystemwatcher looking for log-rotation
    /// </summary>
    public interface IRollingFileWatcherPool : IRollingFileWatcherPoolCommon
    {
        

        /// <summary>
        /// Start for watching on log-files rotation.
        ///
        /// if log4net is not configured or is configured with no <see cref="log4net.Appender.RollingFileAppender"/>
        /// the procedure does nothing and set <see cref="IRollingFileWatcherPoolCommon.Watching"/> to False, otherwise set Watching to True.
        /// </summary>
        /// <param name="newDirectoryPathForZip">The new directory path for zip (Default NULL, use Log Output Directory).</param>
        /// <returns></returns>
        IRollingFileWatcherPool StartWatch(string newDirectoryPathForZip = "");

        IRollingFileWatcherPool DebugEnabled(bool value);

    }
}
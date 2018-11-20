using System;

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
    }


    /// <summary>
    /// A pool of filesystemwatcher looking for log-rotation
    /// </summary>
    public interface IRollingFileWatcherPool
    {
        /// <summary>
        /// If disposed
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// if watching for rotation:
        /// remeber to start watching using <see cref="StartWatch"/> method.
        /// </summary>
        bool Watching { get; }

        void Dispose();

        /// <summary>
        /// Start for watching on log-files rotation.
        ///
        /// if log4net is not configured or is configured with no <see cref="log4net.Appender.RollingFileAppender"/>
        /// the procedure does nothing and set <see cref="Watching"/> to False, otherwise set <see cref="Watching"/> to True.
        /// </summary>
        /// <returns>instance of watcher pool</returns>
        IRollingFileWatcherPool StartWatch();

        event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
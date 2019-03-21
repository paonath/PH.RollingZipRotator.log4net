﻿using System;

namespace PH.RollingZipRotatorLog4net
{
    /// <summary>
    /// A pool of filesystemwatcher looking for log-rotation
    /// </summary>
    public interface IRollingFileWatcherPool : IDisposable
    {
        /// <summary>
        /// If disposed
        /// </summary>
        bool Disposed { get; }

        bool Debug { get; }

        /// <summary>
        /// if watching for rotation:
        /// remeber to start watching using <see cref="StartWatch"/> method.
        /// </summary>
        bool Watching { get; }

        //void Dispose();

        /// <summary>
        /// Start for watching on log-files rotation.
        ///
        /// if log4net is not configured or is configured with no <see cref="log4net.Appender.RollingFileAppender"/>
        /// the procedure does nothing and set <see cref="Watching"/> to False, otherwise set <see cref="Watching"/> to True.
        /// </summary>
        /// <returns>instance of watcher pool</returns>
        IRollingFileWatcherPool StartWatch();

        IRollingFileWatcherPool DebugEnabled(bool value);


        event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
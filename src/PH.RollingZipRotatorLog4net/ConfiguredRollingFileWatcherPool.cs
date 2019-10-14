using System;

namespace PH.RollingZipRotatorLog4net
{
    internal class ConfiguredRollingFileWatcherPool : IConfiguredRollingFileWatcherPool
    {
        private readonly RollingFileWatcherConfig _config;
        private IRollingFileWatcherPool _rollingPool;


        public ConfiguredRollingFileWatcherPool(RollingFileWatcherConfig config)
        {
            _config = config;


            _rollingPool =
                new SimpleRollingFileWatcherPool(_config.AppenderNamesToExclude).DebugEnabled(_config.DebugEnabled);

            _rollingPool.LogRotated += RollingPoolOnLogRotated;

            Disposed = false;
        }

        private void RollingPoolOnLogRotated(object sender, ZipRotationPerformedEventArgs e)
        {
            LogRotated?.Invoke(sender, e);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                _rollingPool?.Dispose();
            }
        }

        /// <summary>
        /// If disposed
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IRollingFileWatcherPoolCommon"/> is debug.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        public bool Debug => _rollingPool.Debug;

        /// <summary>
        /// if watching for rotation:
        /// remeber to start watching using StartWatch method.
        /// </summary>
        public bool Watching => _rollingPool.Watching;

        /// <summary>Occurs when log rotated.</summary>
        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;

        /// <summary>Starts the watch.</summary>
        /// <returns></returns>
        public IRollingFileWatcherPool StartWatch()
        {
            return _rollingPool.StartWatch(_config.OverrideDirectoryPathForZip);
            
        }
    }
}
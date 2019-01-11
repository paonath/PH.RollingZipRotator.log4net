using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;

namespace PH.RollingZipRotatorLog4net
{
    internal class SimpleRollingFileWatcherPool : IDisposable, IRollingFileWatcherPool
    {
        public bool Disposed { get; protected set; }
        public bool Debug { get; protected set; }
        public bool Watching { get; protected set; }

        private readonly List<IRollingFileWatcher> _fileWatchers;

        private ILog _log;

        public SimpleRollingFileWatcherPool()
        {
            Disposed      = false;
            Watching      = false;
            _fileWatchers = new List<IRollingFileWatcher>();
        }
       

        public IRollingFileWatcherPool StartWatch()
        {
            

            var repo = log4net.LogManager.GetAllRepositories();
            foreach (var repository in repo)
            {
                var appenders = repository?.GetAppenders();
                if (appenders != null)
                {
                    foreach (var appender in appenders)
                    {
                        if (appender is RollingFileAppender r)
                        {
                            var path = new FileInfo(r.File);
                            var dir  = path.Directory;
                            if (null != dir && dir.Exists)
                            {
                                var check = _fileWatchers.FirstOrDefault(x => x.DirectoryName == dir.FullName);
                                if (null == check)
                                {
                                    var w = new SimpleRollingFileWatcher(path, _log);
                                    w.LogRotated += WOnLogRotated;
                                    w.Watch();
                                    _fileWatchers.Add(w);
                                    Watching = true;
                                }

                            }

                            
                        }
                    }
                }
            }

            

            return this;
        }

        public IRollingFileWatcherPool DebugEnabled(bool value)
        {
            Debug = value;
            if(Debug == true )
                _log = LogManager.GetLogger(typeof(SimpleRollingFileWatcherPool));
            return this;
        }

        private void WOnLogRotated(object sender, ZipRotationPerformedEventArgs e)
        {
            _log?.Debug($"Log rotated: {e.ZipFile}");

            LogRotated?.Invoke(this, e);
        }

        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
        
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                foreach (var rollingFileWatcher in _fileWatchers)
                {
                    rollingFileWatcher?.Dispose();
                }
                Watching = false;
            }

            Disposed = true;
        }
    }
}
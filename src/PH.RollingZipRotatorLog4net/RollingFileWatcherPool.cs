using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net.Appender;

namespace PH.RollingZipRotatorLog4net
{
    internal class RollingFileWatcherPool : IDisposable, IRollingFileWatcherPool
    {
        private readonly List<IRollingFileWatcher> _fileWatchers;

        private readonly TimeSpan _timeSpanZipRotate;
        private readonly TimeSpan _timeSpanZipArchiveRotate;

        public bool Disposed { get; protected set; }
        public bool Debug { get; }
        public bool Watching { get; private set; }

        public RollingFileWatcherPool(TimeSpan timeSpanZipRotate, TimeSpan timeSpanZipArchiveRotate)
        {
            if (timeSpanZipRotate > timeSpanZipArchiveRotate)
            {
                throw new ArgumentException("Log rotation must be smaller than archive rotation",
                                            nameof(timeSpanZipRotate),
                                            new ArgumentException("Archive rotation must be greather than log rotation",
                                                                  nameof(timeSpanZipArchiveRotate)));
            }

            _timeSpanZipRotate        = timeSpanZipRotate;
            _timeSpanZipArchiveRotate = timeSpanZipArchiveRotate;
            _fileWatchers             = new List<IRollingFileWatcher>();
            Disposed                  = false;
            Watching                  = false;
        }

        public IRollingFileWatcherPool StartWatch()
        {
            

            var repo = log4net.LogManager.GetAllRepositories();
            foreach (var repository in repo)
            {
                if (null != repository)
                {
                    var appenders = repository.GetAppenders();
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
                                        var w = new RollingFileWatcher(path, _timeSpanZipRotate,
                                                                       _timeSpanZipArchiveRotate);
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
            }

            

            return this;
        }

        public IRollingFileWatcherPool DebugEnabled(bool value)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;

        private void WOnLogRotated(object sender, ZipRotationPerformedEventArgs e)
        {
            LogRotated?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                foreach (var rollingFileWatcher in _fileWatchers)
                {
                    rollingFileWatcher?.Dispose();
                }
            }

            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
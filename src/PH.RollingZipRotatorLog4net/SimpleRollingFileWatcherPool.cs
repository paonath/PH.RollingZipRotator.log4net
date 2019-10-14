using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
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
        private readonly string[] _appendersToExclude;


        private ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleRollingFileWatcherPool"/> class.
        /// </summary>
        /// <param name="appenderNamesToExclude">The appender names to exclude.</param>
        public SimpleRollingFileWatcherPool([CanBeNull] IEnumerable<string> appenderNamesToExclude = null)
        {
            Disposed      = false;
            Watching      = false;
            _fileWatchers = new List<IRollingFileWatcher>();
            _appendersToExclude = appenderNamesToExclude?.ToArray() ?? new string[0];

        }
       

        public IRollingFileWatcherPool StartWatch(string newDirectoryPathForZip = "")
        {
            
            var repo = log4net.LogManager.GetAllRepositories();
            foreach (var repository in repo)
            {
                var appenders = repository?.GetAppenders().Where(x => !_appendersToExclude.Contains(x.Name)).ToArray();

                if (appenders != null)
                {
                    foreach (var appender in appenders)
                    {
                        if (appender is RollingFileAppender r)
                        {

                            var path = new FileInfo(r.File);
                            
                            var dir  = path.Directory;
                            if (dir?.Exists == true)
                            {

                                var check = _fileWatchers.FirstOrDefault(x => x.DirectoryName == dir.FullName);
                                if (null == check)
                                {
                                    var otherFileToCompress = path.Directory?.GetFiles();

                                    var w = new SimpleRollingFileWatcher(path, _log, newDirectoryPathForZip);
                                    if (null != otherFileToCompress)
                                    {
                                        foreach (var fileInfo in otherFileToCompress)
                                        {
                                            if (fileInfo.Exists &&
                                                !(fileInfo.Extension.Contains("zip") || fileInfo.Extension.Contains("log")))
                                            {
                                                w.AddToQueueForRotation(fileInfo);
                                            }
                                        }
                                    }
                                    


                                    
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
            {
                _log = LogManager.GetLogger(typeof(SimpleRollingFileWatcherPool));
            }

            return this;
        }

        private void WOnLogRotated(object sender, ZipRotationPerformedEventArgs e)
        {
            _log?.Trace($"Log rotated: {e.ZipFile}");

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
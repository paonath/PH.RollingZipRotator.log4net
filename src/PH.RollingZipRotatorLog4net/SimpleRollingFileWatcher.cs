using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using log4net;
using CompressionLevel = Ionic.Zlib.CompressionLevel;

namespace PH.RollingZipRotatorLog4net
{
    internal class SimpleRollingFileWatcher : IRollingFileWatcher
    {
        private FileInfo _logFileInfo;
        private readonly DirectoryInfo _directory;
        private FileSystemWatcher _watcher;
        private readonly Queue<string> _zipQueue;
        private readonly ILog _log;


        public SimpleRollingFileWatcher([NotNull] FileInfo logFileInfo, [CanBeNull] ILog log)
        {
            _logFileInfo = logFileInfo;
            _log         = log;
            _directory   = logFileInfo.Directory;
            Disposed     = false;
            _zipQueue    = new Queue<string>();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                _log?.Debug("Watcher disposing");
                _watcher?.Dispose();
            }

            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [NotNull]
        public string DirectoryName => _directory.FullName;
        public bool Disposed { get; protected set; }

        [NotNull]
        public IRollingFileWatcher Watch()
        {
            if (!_directory.Exists)
            {
                _directory.Create();
            }


            _watcher = new FileSystemWatcher(_directory.FullName);

            _watcher.Created += WatcherOnCreated;
            _watcher.Renamed += WatcherOnRenamed;


            if(_zipQueue.Count > 0)
            {
                LogCompress("Initial compress on start");
            }

            _watcher.EnableRaisingEvents = true;

            _log?.Debug($"watching on '{_directory.FullName}'");

            return this;
        }


        internal void AddToQueueForRotation([NotNull] FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Exists)
            {
                _zipQueue.Enqueue(file.FullName);
            }
        }

        private void WatcherOnRenamed(object sender, [NotNull] RenamedEventArgs e)
        {
            if (!e.FullPath.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                var patternName = _logFileInfo.Name.Replace(_logFileInfo.Extension, "");
                if (e.Name.Contains(patternName))
                {
                    _zipQueue.Enqueue(e.FullPath);
                }
            }
        }

        private void WatcherOnCreated(object sender, [NotNull] FileSystemEventArgs e)
        {
            if (!e.FullPath.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                LogCompress();
            }
        }

        private readonly object _lck = new object();

        private void LogCompress([CanBeNull] string message = "")
        {
            if(!string.IsNullOrEmpty(message))
            {
                _log?.Debug(message);
            }

            try
            {
                lock (_lck)
                {
                    var d     = DateTime.Now;
                    var dDay  = $"{d:yyyy-MM-dd}";
                    var dTime = $"{d:HH-mm-ss}";

                    var outDir = new DirectoryInfo($"{_directory.FullName}{Path.DirectorySeparatorChar}{dDay}");
                    if (!outDir.Exists)
                    {
                        outDir.Create();
                    }

                    var            zipFile = new FileInfo($"{outDir.FullName}{Path.DirectorySeparatorChar}{dDay}.zip");
                    


                    var zipper = new NewZipper();
                    var l      = new Dictionary<string, FileInfo>();


                    int c = 0;
                    while (_zipQueue.Count > 0)
                    {
                        c++;
                        FileInfo file = new FileInfo(_zipQueue.Dequeue());

                        if (file.Exists)
                        {
                            var entryName = $"{dDay}_{dTime}__{file.Name}_{c}.log";
                            l.Add(entryName, file);
                        }
                    }

                    if (l.Count > 0)
                    {
                        zipper.AddEntries(l, zipFile.FullName, CompressionLevel.BestCompression );
                        LogRotated?.Invoke(this, new ZipRotationPerformedEventArgs() {ZipFile = zipFile.FullName});
                    }
                }
            }
            catch (Exception exception)
            {
                _log?.Debug($"LogCompress exception '{exception.Message}'", exception);
                //
            }
        }

        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
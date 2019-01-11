using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;
using log4net;

namespace PH.RollingZipRotatorLog4net
{
    internal class SimpleRollingFileWatcher : IRollingFileWatcher
    {
        private FileInfo _logFileInfo;
        private readonly DirectoryInfo _directory;
        private FileSystemWatcher _watcher;
        private Queue<string> _zipQueue;
        private readonly ILog _log;


        public SimpleRollingFileWatcher([NotNull] FileInfo logFileInfo, [CanBeNull] ILog log)
        {
            _logFileInfo = logFileInfo;
            _log = log;
            _directory = logFileInfo.Directory;
            Disposed = false;
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

        public string DirectoryName => _directory.Name;
        public bool Disposed { get; protected set; }

        public IRollingFileWatcher Watch()
        {
            if(!_directory.Exists)
                _directory.Create();

            _zipQueue = new Queue<string>();

            _watcher = new FileSystemWatcher(_directory.FullName);

            _watcher.Created += WatcherOnCreated;
            _watcher.Renamed += WatcherOnRenamed;


            _watcher.EnableRaisingEvents = true;

            _log?.Debug($"watching on '{_directory.FullName}'");

            return this;
        }

        private void WatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            if (!e.FullPath.EndsWith(".zip"))
            {
                var patternName = _logFileInfo.Name.Replace(_logFileInfo.Extension, "");
                if(e.Name.Contains(patternName))
                    _zipQueue.Enqueue(e.FullPath);

            }

        }

        private void WatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.EndsWith(".zip"))
            {
                try
                {
                    LogCompress();
                }
                catch (Exception exception)
                {
                    _log?.Debug($"LogCompress exception '{exception.Message}'", exception);
                    //
                }
            }
        }

        private object lck = new object();

        private void LogCompress()
        {
            lock (lck)
            {
                var d     = DateTime.Now;
                var dDay  = $"{d:yyyy-MM-dd}";
                var dTime = $"{d:HH-mm-ss}";

                var outDir = new DirectoryInfo($"{_directory.FullName}{Path.DirectorySeparatorChar}{dDay}");
                if (!outDir.Exists)
                    outDir.Create();

                var            zipFile = new FileInfo($"{outDir.FullName}{Path.DirectorySeparatorChar}{dDay}.zip");
                ZipArchiveMode mode    = ZipArchiveMode.Create;
                if (zipFile.Exists)
                    mode = ZipArchiveMode.Update;


                var zipper = new Zipper();
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
                    zipper.AddEntries(l, zipFile.FullName, mode, CompressionLevel.Optimal);
                    LogRotated?.Invoke(this, new ZipRotationPerformedEventArgs() {ZipFile = zipFile.FullName});
                }

            }
        }

        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
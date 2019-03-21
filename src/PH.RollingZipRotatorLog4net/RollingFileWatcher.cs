using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using log4net.Repository.Hierarchy;

namespace PH.RollingZipRotatorLog4net
{
    internal class RollingFileWatcher : IDisposable, IRollingFileWatcher
    {
        private readonly DirectoryInfo _directory;
        private readonly TimeSpan _timeSpanZipRotate;
        private readonly TimeSpan _timeSpanZipArchiveRotate;
        private readonly Zipper _zipper;
        private FileSystemWatcher _watcher;
        private readonly Queue<string> _zipQueue;

        public bool Disposed { get; protected set; }
        [CanBeNull]
        public string DirectoryName => _directory?.FullName;
        private readonly FileInfo _logFileInfo;
        
        public RollingFileWatcher(FileInfo logFileInfo, TimeSpan timeSpanZipRotate, TimeSpan timeSpanZipArchiveRotate)
        {
            _logFileInfo = logFileInfo;
            _timeSpanZipRotate = timeSpanZipRotate;
            _timeSpanZipArchiveRotate = timeSpanZipArchiveRotate;

            if (_timeSpanZipRotate > _timeSpanZipArchiveRotate)
                throw new ArgumentException("Log rotation must be smaller than archive rotation",
                                            nameof(timeSpanZipRotate),
                                            new ArgumentException("Archive rotation must be greather than log rotation",
                                                                  nameof(timeSpanZipArchiveRotate)));


            _directory = logFileInfo.Directory;
            _zipper = new Zipper();

            _zipper.LogRotated += ZipperOnLogRotated;

            _zipQueue = new Queue<string>();
            Disposed = false;
        }

        private void ZipperOnLogRotated(object sender, ZipRotationPerformedEventArgs e)
        {
            LogRotated?.Invoke(this, e);
        }


        public IRollingFileWatcher Watch()
        {
            if (!_directory.Exists)
                _directory.Create();

            _watcher = new FileSystemWatcher(_directory.FullName);

            

            CheckAndZipOnStart();

            _watcher.Created += WatcherOnCreated;
            _watcher.Renamed += WatcherOnRenamed;


            _watcher.EnableRaisingEvents = true;

            return this;
        }

        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;

        private void CheckAndZipOnStart()
        {
            var patternName = _logFileInfo.Name.Replace(_logFileInfo.Extension, "");

            var files = _directory.GetFiles().Where(x => x.Extension != ".log" && x.Name.Contains(patternName) && x.Extension != ".zip")
                                  .Select(x => x.FullName).ToList();
            try
            {
                LogCompressMultipleInitial(files);
            }
            catch(Exception e)
            {
               
            }
            
        }
        private void LogCompressMultipleInitial(List<string> filesinitial)
        {
            if(filesinitial.Count == 0)
                return;



            var files = filesinitial.Select(InitFile.GetFile).Where(x => x != null).ToArray();
            var dates = files.Select(x => x.DateNum).Distinct().OrderBy(x => x).ToArray();
            try
            {
                lock (myLock)
                {
                    List<FileInfo> zipToArchiveList = new List<FileInfo>();
                    foreach (var date in dates)
                    {
                        var filePerDates = files.Where(x => x.DateNum == date).OrderBy(x => x.FileInfo.Name)
                                                .Select(x => x.FileInfo).ToList();

                        //.ToArray();
                        if (filePerDates.Count > 0)
                        {
                            var zipArchive = $"{_directory.FullName}{Path.DirectorySeparatorChar}history_archive_logs_{date}.zip";
                            //using (var zip = ZipFile.Open(zipArchive, ZipArchiveMode.Create ))
                            //{
                            //    foreach (var filePerDate in filePerDates)
                            //    {
                            //        zip.CreateEntryFromFile(filePerDate.FileInfo.FullName, filePerDate.FileInfo.Name, CompressionLevel.Optimal);
                            //        System.IO.File.Delete(filePerDate.FileInfo.FullName);
                            //    }
                            //}

                            _zipper.AddFilesAndDeleteFromDisk(filePerDates, zipArchive, ZipArchiveMode.Create, CompressionLevel.Optimal);

                            zipToArchiveList.Add(new FileInfo(zipArchive));
                        }

                        
                    }



                    var d = DateTime.Now;
                    var iso = $"{d:O}".Replace(":", "").Replace("T","_").Replace(".", "").Replace("+","");

                    var zopArchive = $"{_directory.FullName}{Path.DirectorySeparatorChar}history_archive_logs_{iso}.zip";
                    //using (var zop = ZipFile.Open(zopArchive, ZipArchiveMode.Create))
                    //{
                    //    foreach (var c in zipToArchiveList)
                    //    {
                    //        zop.CreateEntryFromFile(c.FullName, c.Name, CompressionLevel.NoCompression);
                    //        c.Delete();
                    //    }
                
                    //}

                    _zipper.AddFilesAndDeleteFromDisk(zipToArchiveList, zopArchive, ZipArchiveMode.Create,
                                                      CompressionLevel.NoCompression);
            
                    File.SetAttributes(zopArchive, FileAttributes.Archive | FileAttributes.ReadOnly);

                }
                
            }
            catch 
            {
                //
            }
            

        }

        private string GetNewZipArchiveNameForRotating(DateTime dt, FileInfo zipArchive, int appendNumber = 0)
        {
            
            var n = zipArchive.Name.Replace(".zip", $"-{dt:hh-mm}-{appendNumber}.zip");
            if (System.IO.File.Exists($"{_directory.FullName}{Path.DirectorySeparatorChar}{n}"))
            {
                appendNumber++;
                return GetNewZipArchiveNameForRotating(dt, zipArchive, appendNumber);
            }
            else
                return n;

        }

        private ZipArchiveMode GetModeFromZip(FileInfo zipArchive)
        {
            if (!zipArchive.Exists)
                return ZipArchiveMode.Create;

            var creationTime = zipArchive.CreationTime;
            var rotateTime = creationTime.Add(_timeSpanZipRotate);
            var dt = DateTime.Now;
            if(rotateTime < dt)
            {
                var newZipName = GetNewZipArchiveNameForRotating(dt, zipArchive);
                
                var zipPath = $"{_directory.FullName}{Path.DirectorySeparatorChar}{newZipName}";
                zipArchive.MoveTo(zipPath);

                return ZipArchiveMode.Create;

            }

            return ZipArchiveMode.Update;
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
        
        private object myLock = new object();

        private void PerformZipArchiveRotate(FileInfo[] zipToArchiveArray)
        {
            var d = DateTime.Now;
            var iso = $"{d:O}".Replace(":", "").Replace("T","_").Replace(".", "").Replace("+","");
            var zipArchive = $"{_directory.FullName}{Path.DirectorySeparatorChar}archive_logs_{iso}.zip";
            
            
            //using (var zop = ZipFile.Open(zipArchive, ZipArchiveMode.Create))
            //{
            //    foreach (var c in zipToArchiveArray)
            //    {
            //        zop.CreateEntryFromFile(c.FullName, c.Name, CompressionLevel.NoCompression);
            //        c.Delete();
            //    }
                
            //}

            _zipper.AddFilesAndDeleteFromDisk(zipToArchiveArray.ToList(), zipArchive, ZipArchiveMode.Create, CompressionLevel.NoCompression);
            

            var attrs = System.IO.File.GetAttributes(zipArchive);



            File.SetAttributes(zipArchive, FileAttributes.Archive | FileAttributes.ReadOnly);




        }

        private void LogCompressMultiple()
        {
            try
            {
                lock (myLock)
                {
                    if (_zipQueue.Count > 0)
                    {
                        var d   = DateTime.Now;
                        var iso = $"{d:O}".Replace(":", "").Replace(".", "").Replace("+","");
                    
                        var zipArchive = $"{_directory.FullName}{Path.DirectorySeparatorChar}logs_{d:yy-MM-dd}.zip";

                

                        var zipMode = GetModeFromZip(new FileInfo(zipArchive));
                        Dictionary<string,FileInfo> files = new Dictionary<string, FileInfo>();
                        while (_zipQueue.Count > 0)
                        {
                            FileInfo file = new FileInfo(_zipQueue.Dequeue());

                            if (file.Exists)
                            {
                                var entryName = $"{iso}__{file.Name}.log";
                                //zip.CreateEntryFromFile(file.FullName, entryName, CompressionLevel.Optimal);
                                files.Add(entryName, file);

                            }

                            //file.Delete();
                        }

                        _zipper.AddEntries(files, zipArchive, zipMode, CompressionLevel.Optimal);


                        //using (var zip = ZipFile.Open(zipArchive, zipMode))
                        //{
                        //    while (_zipQueue.Count > 0)
                        //    {
                        //        FileInfo file = new FileInfo(_zipQueue.Dequeue());

                        //        if (file.Exists)
                        //        {
                        //            var entryName = $"{iso}__{file.Name}.log";
                        //            zip.CreateEntryFromFile(file.FullName, entryName, CompressionLevel.Optimal);

                        //        }

                        //        file.Delete();
                        //    }   
                        //}


                    }
                }

                DateTime archiveParam = DateTime.Now.Subtract(_timeSpanZipArchiveRotate);

                var f = _directory.GetFiles("*.zip")
                                  .Where(x => x.CreationTime <= archiveParam && !x.Name.StartsWith("archive") ).ToArray();
                if (f.Length > 0)
                {
                    lock (myLock)
                    {
                        PerformZipArchiveRotate(f);
                    }
                }

            }
            catch 
            {
               //
            }
            

        }

        

        private void WatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.EndsWith(".zip"))
            {
                try
                {
                    LogCompressMultiple();
                }
                catch 
                {
                   //
                }
            }

            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                _watcher?.Dispose();
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

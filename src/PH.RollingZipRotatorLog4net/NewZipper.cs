using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using JetBrains.Annotations;

namespace PH.RollingZipRotatorLog4net
{
    public class NewZipper : INewZipper
    {
        private readonly object _zipLock = new object();

        

        public void AddEntries(Dictionary<string, FileInfo> files, string zipArchiveName, Ionic.Zlib.CompressionLevel level)
        {
            try
            {
                if (files.Count > 0)
                {
                    lock (_zipLock)
                    {
                        var filesToDelete = new List<string>();
                        //

                        using (var izip = new Ionic.Zip.ZipFile(zipArchiveName))
                        {
                            izip.CompressionLevel = level;
                            foreach (var keyValuePair in files)
                            {
                                try
                                {
                                    if (keyValuePair.Value.Exists)
                                    {
                                        ZipEntry e = izip.AddFile(keyValuePair.Value.FullName);

                                        e.FileName = keyValuePair.Key;
                                        filesToDelete.Add(keyValuePair.Value.FullName);
                                        
                                    }
                                }
                                catch 
                                {
                                    //
                                }
                                
                            }

                            izip.Save();

                        }

                        foreach (var fileToDelete in filesToDelete)
                        {
                            try
                            {
                                System.IO.File.Delete(fileToDelete);
                            }
                            catch 
                            {
                               
                            }
                        }
                      

                        OnLogRotated(new ZipRotationPerformedEventArgs() {ZipFile = zipArchiveName});
                    }

                }
            }
            catch /*(Exception exception)*/
            {
               //
            }
            
        }

        public void AddEntryToZip(FileInfo f, [NotNull] string entryName, string zipArchiveName,Ionic.Zlib.CompressionLevel level)
        {
            var d = new Dictionary<string, FileInfo>() {{entryName, f}};
            AddEntries(d, zipArchiveName,  level);
        }

        public void AddFilesAndDeleteFromDisk([NotNull] IEnumerable<FileInfo> files, string zipArchiveName, Ionic.Zlib.CompressionLevel level)
        {
            var fileInfos = files as FileInfo[] ?? files.ToArray();
            if (fileInfos.Any())
            {
                var filePerDates = fileInfos.OrderBy(x => x.LastWriteTimeUtc).ToList();
                var d            = new Dictionary<string, FileInfo>();
                foreach (var filePerDate in filePerDates)
                {
                    d.Add(filePerDate.Name, filePerDate);
                }
                
                AddEntries(d,zipArchiveName, level);
            }
        }

        protected virtual void OnLogRotated(ZipRotationPerformedEventArgs e)
        {
            LogRotated?.Invoke(this, e);
        }

        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;

//namespace PH.RollingZipRotatorLog4net
//{
//    public class Zipper : IZipper
//    {
//        private object zipLock = new object();

        

//        public void AddEntries(Dictionary<string, FileInfo> files, string zipArchiveName, ZipArchiveMode mode,
//                               CompressionLevel level)
//        {
//            try
//            {
//                if (files.Count > 0)
//                {
//                    lock (zipLock)
//                    {
//                        //


//                        using (var zip = ZipFile.Open(zipArchiveName, mode ))
//                        {
//                            foreach (var keyValuePair in files)
//                            {
//                                try
//                                {
//                                    if (keyValuePair.Value.Exists)
//                                    {
//                                        zip.CreateEntryFromFile(keyValuePair.Value.FullName, keyValuePair.Key, level);
//                                        try
//                                        {
//                                            System.IO.File.Delete(keyValuePair.Value.FullName);
//                                        }
//                                        catch (Exception e)
//                                        {
//                                            //
//                                        }
//                                    }
//                                }
//                                catch (Exception e)
//                                {
//                                   //
//                                }
                                
//                            }
                    
//                        }

//                        OnLogRotated(new ZipRotationPerformedEventArgs() {ZipFile = zipArchiveName});
//                    }

//                }
//            }
//            catch (Exception exception)
//            {
               
//            }
            
//        }

//        public void AddEntryToZip(FileInfo f, string entryName, string zipArchiveName, ZipArchiveMode mode,
//                                  CompressionLevel level)
//        {
//            var d = new Dictionary<string, FileInfo>() {{entryName, f}};
//            AddEntries(d, zipArchiveName, mode, level);
//        }

//        public void AddFilesAndDeleteFromDisk(List<FileInfo> files, string zipArchiveName, ZipArchiveMode mode, CompressionLevel level)
//        {
//            if (files.Count > 0)
//            {
//                var filePerDates = files.OrderBy(x => x.LastWriteTimeUtc).ToList();
//                var d            = new Dictionary<string, FileInfo>();
//                foreach (var filePerDate in filePerDates)
//                {
//                    d.Add(filePerDate.Name, filePerDate);
//                }
                
//                AddEntries(d,zipArchiveName, mode, level);
//            }

            
//        }

//        protected virtual void OnLogRotated(ZipRotationPerformedEventArgs e)
//        {
//            LogRotated?.Invoke(this, e);
//        }

//        public event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
//    }
//}
using System;
using System.Collections.Generic;
using System.IO;


namespace PH.RollingZipRotatorLog4net
{
    public interface INewZipper
    {
        void AddEntries(Dictionary<string, FileInfo> files, string zipArchiveName,  Ionic.Zlib.CompressionLevel level);
        void AddEntryToZip(FileInfo f, string entryName, string zipArchiveName,  Ionic.Zlib.CompressionLevel level);
        void AddFilesAndDeleteFromDisk(IEnumerable<FileInfo> files, string zipArchiveName,  Ionic.Zlib.CompressionLevel level);

        event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
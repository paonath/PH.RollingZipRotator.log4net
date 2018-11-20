using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace PH.RollingZipRotatorLog4net
{
    public interface IZipper
    {
        void AddEntries(Dictionary<string, FileInfo> files, string zipArchiveName, ZipArchiveMode mode, CompressionLevel level);
        void AddEntryToZip(FileInfo f, string entryName, string zipArchiveName, ZipArchiveMode mode, CompressionLevel level);
        void AddFilesAndDeleteFromDisk(List<FileInfo> files, string zipArchiveName, ZipArchiveMode mode, CompressionLevel level);

        event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
using System;
using System.Collections.Generic;

namespace PH.RollingZipRotatorLog4net
{
    /// <summary>
    /// On Created new Zip rotation file
    /// </summary>
    public class ZipRotationPerformedEventArgs : EventArgs
    {
        /// <summary>
        /// Created zip-file with logs rotated
        /// </summary>
        public string ZipFile { get; set; }

        public List<string> FileDeleted { get; set; }
        public List<string> EntryAdded { get; set; }

        public long ZipSize { get; set; }
        public DateTime UtcFired { get; private set; }

        internal ZipRotationPerformedEventArgs()
        {
            FileDeleted = new List<string>(); 
            EntryAdded = new List<string>(); 
            UtcFired = DateTime.UtcNow;
            

        }

        public ZipRotationPerformedEventArgs(string zipFile, List<string> added, List<string> deleted,long zipSize)
            :this()
        {
            ZipFile = zipFile;
            EntryAdded = added;
            FileDeleted = deleted;
            ZipSize = zipSize;

        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"ZipFile '{ZipFile}' - Size {ZipSize} - Added {EntryAdded.Count} - Deleted {EntryAdded.Count}";
        }
    }
}
using System;

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
    }
}
using System;

namespace PH.RollingZipRotatorLog4net
{

    public interface IRollingFileWatcher : IDisposable
    {
        string DirectoryName { get; }
        bool Disposed { get; }

        
        IRollingFileWatcher Watch();

        event EventHandler<ZipRotationPerformedEventArgs> LogRotated;
    }
}
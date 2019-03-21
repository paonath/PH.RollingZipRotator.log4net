using JetBrains.Annotations;

namespace PH.RollingZipRotatorLog4net
{
    public static class RollingFileFactory
    {
        
        [NotNull]
        public static IRollingFileWatcherPool CreateSimple()
        {
            var r = new SimpleRollingFileWatcherPool();
            return r;
        }

    }
}
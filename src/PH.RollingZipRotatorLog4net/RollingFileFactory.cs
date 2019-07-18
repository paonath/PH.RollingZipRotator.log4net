using System.Collections.Generic;
using JetBrains.Annotations;

namespace PH.RollingZipRotatorLog4net
{
    public static class RollingFileFactory
    {
        
        [NotNull]
        public static IRollingFileWatcherPool CreateSimple([CanBeNull] IEnumerable<string> appenderNamesToExclude = null)
        {
            var r = new SimpleRollingFileWatcherPool( appenderNamesToExclude);
            return r;
        }
        [NotNull]
        public static IRollingFileWatcherPool CreateSimple(string appenderNameToExclude)
        {
            var r = new SimpleRollingFileWatcherPool(new string[]{appenderNameToExclude});
            return r;
        }

    }
}
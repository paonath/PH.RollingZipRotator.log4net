using System;
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


        /// <summary>Read the configuration from file and return instance.</summary>
        /// <param name="configXmlPath">The configuration XML path.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Unable to read config from {configXmlPath}</exception>
        [NotNull]
        public static IConfiguredRollingFileWatcherPool GetConfig(string configXmlPath)
        {
            var cfg = RollingFileWatcherConfig.ReadFromFile(configXmlPath);
            if (null == cfg)
            {
                throw new ArgumentException($"Unable to read config from {configXmlPath}", configXmlPath);
            }

            return SetConfig(cfg);
        }

        /// <summary>Sets the configuration.</summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static IConfiguredRollingFileWatcherPool SetConfig(RollingFileWatcherConfig config)
        {
            var r = new ConfiguredRollingFileWatcherPool(config);
            return r;
        }
    }
}
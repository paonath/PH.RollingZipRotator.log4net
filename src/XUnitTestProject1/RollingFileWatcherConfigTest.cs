using System.Collections.Generic;
using PH.RollingZipRotatorLog4net;
using Xunit;

namespace XUnitTestProject1
{
    public class RollingFileWatcherConfigTest : BaseTest
    {
        [Fact]
        public void SerializeAConfig()
        {
            RollingFileWatcherConfig cfg = new RollingFileWatcherConfig()
            {
                DebugEnabled                = true,
                OverrideDirectoryPathForZip = @"C:\temp\pippo",
                AppenderNamesToExclude = new []{"fullLog"}
            };

            cfg.SaveToFile(@"C:\temp\cfg.config");



        }

        [Fact]
        public void REadConfig()
        {
            var cfg = RollingFileWatcherConfig.ReadFromFile(@"C:\temp\cfg.config");

            Assert.NotNull(cfg);
        }


        [Fact]
        public void Perform_A_RotationExcludingSomeAppender()
        {

            var logger   = GetALog();
            int rotation = 0;

            var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.GetConfig(@"C:\temp\cfg.config");

            
            instance.LogRotated += (sender, args) =>
            {
                rotation++;
            };
            

            instance.StartWatch();

            var isInstanceWatching = instance.Watching;


            while (rotation < 10)
            {
                var msg = "Write some data";
                logger.Info(msg);


                
                logger.Debug(msg);
                logger.Error(msg);
                logger.Fatal(msg);
                logger.Warn(msg);
                //System.Threading.Thread.Sleep(150);
            }

            Assert.True(rotation > 0);
            Assert.True(isInstanceWatching);
        }
    }
}
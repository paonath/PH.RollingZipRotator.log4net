using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using PH.RollingZipRotatorLog4net;
using Xunit;

namespace XUnitTestProject1
{
     public abstract class BaseTest
    {
        public void ConfigureL4net()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var configFile    = new FileInfo("log4net.config");

            XmlConfigurator.Configure(logRepository, configFile);

            ILog log = LogManager.GetLogger(typeof(BaseTest));
            log.Info("Init test");


        }

        public ILog GetALog()
        {
            ConfigureL4net();

            var l = LogManager.GetLogger(typeof(BaseTest));


            return l;

        }


    }

    public class RollingFileTest : BaseTest
    {
        [Fact]
        public void Factory_Return_NotNullInstance()
        {
            var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();

            Assert.NotNull(instance);
            Assert.True((instance as IRollingFileWatcherPool) != null);
        }


        [Fact]
        public void Perform_A_Rotation()
        {

            var logger = GetALog();
            int rotation = 0;

            var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();
            
            instance.LogRotated += (sender, args) =>
            {
                rotation++;
            };

            instance.DebugEnabled(true);

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
                System.Threading.Thread.Sleep(150);
            }

            Assert.True(rotation > 0);
            Assert.True(isInstanceWatching);
        }

        [Fact]
        public void Start_Watcher_WithNoLog_DoNotPerformCode()
        {
            var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();

            instance.StartWatch();
            var isInstanceWatching = instance.Watching;

            Assert.False(isInstanceWatching == true);

        }
    }
    //public class RollingFileTest : BaseTest
    //{
    //    [Fact]
    //    public void Factory_Return_NotNullInstance()
    //    {
    //        var instance =
    //            PH.RollingZipRotatorLog4net.RollingFileFactory.Create(new TimeSpan(0, 1, 0), new TimeSpan(0, 2, 0));

    //        Assert.NotNull(instance);
    //        Assert.True((instance as IRollingFileWatcherPool) != null);
    //    }


    //    [Fact]
    //    public void Perform_A_Rotation()
    //    {

    //        var logger = GetALog();

    //        var instance =
    //            PH.RollingZipRotatorLog4net.RollingFileFactory.Create(new TimeSpan(0, 1, 0), new TimeSpan(0, 2, 0));

    //        int rotation = 0;
    //        instance.LogRotated += (sender, args) =>
    //        {
    //            rotation++;
    //        };

    //        instance.StartWatch();

    //        var isInstanceWatching = instance.Watching;


    //        while (rotation < 10)
    //        {
    //            var msg = "Write some data";
    //            logger.Info(msg);


                
    //            logger.Debug(msg);
    //            logger.Error(msg);
    //            logger.Fatal(msg);
    //            logger.Warn(msg);

    //        }

    //        Assert.True(rotation > 0);
    //        Assert.True(isInstanceWatching);
    //    }

    //    [Fact]
    //    public void Start_Watcher_WithNoLog_DoNotPerformCode()
    //    {
    //        var instance =
    //            PH.RollingZipRotatorLog4net.RollingFileFactory.Create(new TimeSpan(0, 1, 0), new TimeSpan(0, 2, 0));

    //        instance.StartWatch();
    //        var isInstanceWatching = instance.Watching;

    //        Assert.False(isInstanceWatching == true);

    //    }
    //}
}


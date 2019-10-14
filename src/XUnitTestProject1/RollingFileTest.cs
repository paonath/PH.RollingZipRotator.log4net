using System.IO;
using PH.RollingZipRotatorLog4net;
using Xunit;

namespace XUnitTestProject1
{
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
                var s = sender;
                var a = args;

                rotation++;
            };

            instance.DebugEnabled(true);

            instance.StartWatch(@"C:\temp\debug_today");

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


        
        [Fact]
        public void Perform_A_RotationExcludingSomeAppender()
        {

            var logger   = GetALog();
            int rotation = 0;

            var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple("fullLog");
            
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
                //System.Threading.Thread.Sleep(150);
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

        [Fact]
        public void Starting_Watcher_On_Not_Empty_Di_rPerform_Automatic_Rotation_Over_Other_Files()
        {
            var logger = GetALog();

            var dir = new DirectoryInfo(@".\log");
            for (int i = 0; i < 10; i++)
            {
                FileInfo f = new FileInfo($"{dir.FullName}{Path.DirectorySeparatorChar}Fakelog.log.{i}");
                f.Create();
                
            }



            var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();

            instance.StartWatch();
            var isInstanceWatching = instance.Watching;
            Assert.NotNull(instance);
            Assert.True((instance as IRollingFileWatcherPool) != null);
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


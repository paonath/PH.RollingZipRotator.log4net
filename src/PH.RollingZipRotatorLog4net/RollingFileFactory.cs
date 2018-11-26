namespace PH.RollingZipRotatorLog4net
{
    public static class RollingFileFactory
    {

        /*
        public static IRollingFileWatcherPool Create(TimeSpan timeSpanZipRotate, TimeSpan timeSpanZipArchiveRotate)
        {
            var r = new RollingFileWatcherPool(timeSpanZipRotate, timeSpanZipArchiveRotate);
            return r;
        }
        */

        public static IRollingFileWatcherPool CreateSimple()
        {
            var r = new SimpleRollingFileWatcherPool();
            return r;
        }

    }
}
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

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
}
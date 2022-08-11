using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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


        public string ProvideFile(string filePath, [CallerMemberName] string memberName = "")
        {
	        FileInfo f = new FileInfo(filePath);
	        if (!f.Exists)
	        {
		        StringBuilder builder = new StringBuilder();
		        builder.AppendLine($"Demo File for {memberName} : {DateTime.Now:O} - ");
		        
		        for (int i = 0; i < 100; i++)
		        {
			        builder.AppendLine($"just demo {i}");
		        }

            System.IO.File.WriteAllText(f.FullName,builder.ToString());
	        }

	        return f.FullName;

        }

    }
}
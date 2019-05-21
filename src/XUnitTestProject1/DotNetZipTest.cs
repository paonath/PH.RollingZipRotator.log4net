using Ionic.Zip;
using Xunit;

namespace XUnitTestProject1
{
    public class DotNetZipTest : BaseTest
    {
        [Fact]
        public void Test01()
        {

            var filename = @"c:\temp\soffas-fms-uploadftp-sql.log";


            using(ZipFile zip= new ZipFile())
            {
                zip.AddFile(filename);

                zip.Save(".\\test01.zip");
            }
        }

        [Fact]
        public void Test02()
        {

            var filename = @"c:\temp\p1maintenance-sql.log";
            var zipFile = @".\test01.zip";



            using(ZipFile zip= new ZipFile(zipFile))
            {
                
                zip.AddFile(filename);
                zip.Save();
            }
        }


    }
}

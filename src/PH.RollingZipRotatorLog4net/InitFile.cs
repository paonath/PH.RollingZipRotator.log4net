using System.IO;
using JetBrains.Annotations;

namespace PH.RollingZipRotatorLog4net
{
    internal class InitFile
    {
        public FileInfo FileInfo { get; set; }
        public int DateNum { get; set; }

        [CanBeNull]
        public static InitFile GetFile([NotNull] string path)
        {
            System.IO.FileInfo f = new FileInfo(path);
            if (!f.Exists)
            {
                return null;
            }

            var d = f.LastWriteTime;

            int dateNum = int.Parse($"{d:yyyyMMdd}");

            return new InitFile(){ FileInfo = f, DateNum = dateNum};
        }
    }
}
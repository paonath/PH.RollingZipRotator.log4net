using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace PH.RollingZipRotatorLog4net
{
    
    public class RollingFileWatcherConfig
    {
        [XmlAttribute("debug")]
        public bool DebugEnabled { get; set; } = false;

        [XmlAttribute("overrideDirectoryPathForZip")]
        public string OverrideDirectoryPathForZip { get; set; } = "";

        [XmlElement("appenderNamesToExclude")]
        public string[] AppenderNamesToExclude { get; set; } = null;



        public void SaveToFile(string filePath)
        {
            XmlSerializer     serializer = new XmlSerializer(typeof(RollingFileWatcherConfig));
            XmlWriterSettings settings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");
            using (var f = new FileStream(filePath, FileMode.CreateNew))
            {
                serializer.Serialize(f, this, ns);
            }


        }

        private static RollingFileWatcherConfig ReadFromFileInfo(FileInfo fileConfig)
        {
            if (!fileConfig.Exists)
            {
                throw new ArgumentException($"File '{fileConfig.FullName}' does not exists", nameof(fileConfig));
            }

            XmlSerializer           serializer = new XmlSerializer(typeof(RollingFileWatcherConfig));
            XmlWriterSettings       settings   = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};
            XmlSerializerNamespaces ns         = new XmlSerializerNamespaces();
            ns.Add("","");

            using (var f = new FileStream(fileConfig.FullName, FileMode.Open))
            {
                var cfg = (RollingFileWatcherConfig) serializer.Deserialize(f);
                return cfg;
            }
        }

        public static RollingFileWatcherConfig ReadFromFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return ReadFromFileInfo(fi);

        }
    }
}
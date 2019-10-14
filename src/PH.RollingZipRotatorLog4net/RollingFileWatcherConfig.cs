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

        public static RollingFileWatcherConfig ReadFromFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Exists)
            {
                return null;
            }

            XmlSerializer     serializer = new XmlSerializer(typeof(RollingFileWatcherConfig));
            XmlWriterSettings settings   = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            using (var f = new FileStream(filePath, FileMode.Open))
            {
                var cfg = (RollingFileWatcherConfig) serializer.Deserialize(f);
                return cfg;
            }
        }
    }
}
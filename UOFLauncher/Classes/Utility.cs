using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    internal class Utility
    {
        public static object MapXmlFileToClass(Type type, string path)
        {
            var serializer = new XmlSerializer(type);
            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                return serializer.Deserialize(reader);
            }
        }

        public static object MapXmlDocToClass(Type type, XDocument doc)
        {
            var serializer = new XmlSerializer(type);
            using (var reader = doc.CreateReader())
            {
                return serializer.Deserialize(reader);
            }
        }

        public static void MapClassToXmlFile(Type type, object obj, string path)
        {
            var serializer = new XmlSerializer(type);
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, obj);
            }
        }

        public static string Md5Hash(string s)
        {
            var sb = new StringBuilder();
            HashAlgorithm algorithm = MD5.Create();
            var h = algorithm.ComputeHash(Encoding.Default.GetBytes(s));

            foreach (var b in h)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string Md5Checksum(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        public static void CreateFileFromResource(string path, string resource, bool overwrite = false)
        {
            if (!File.Exists(path) || overwrite && File.Exists(path))
            {

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                            {
                                sw.Write(reader.ReadToEnd());
                            }
                        }
                    }
                }
            }

            var configCorrupted = false;

            try
            {
                Config.Instance = ((Config)Utility.MapXmlFileToClass(typeof(Config), Constants.ConfigFilePath));
                Config.Instance.Initiated = true;
            }
            catch (Exception)
            {
                configCorrupted = true;
            }
        }

        public static string GetProgramFilesDirectory()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public static int VersionToInt(Version version)
        {
            return version.Major * 10000000 + version.Minor * 10000 + version.Build * 100 + version.Revision;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class Launcher
    {
        private static LauncherUdateObj _launcherUpdate;

        public static async Task<bool> CheckForUpdate()
        {
            DeleteOldUpdates();

            var doc = await Updates.GetUpdates(Constants.LauncherDownload);

            if (doc == null)
                return false;

            try
            {
                _launcherUpdate = ((LauncherUdateObj) Utility.MapXmlDocToClass(typeof (LauncherUdateObj), doc));

                if (CompareVersions(_launcherUpdate.Version))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static async Task BeginUpdates()
        {
            var filepath = Path.Combine(Constants.AppDirectory, _launcherUpdate.FileName);

            //if update file already exists, just execute it
            if (File.Exists(filepath))
                ExecuteFile();
            else
            {
                try
                {
                    if (
                        await
                            _launcherUpdate.Download(filepath
                                ))
                    {
                        ExecuteFile();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private static void ExecuteFile()
        {
            var filepath = Path.Combine(Constants.AppDirectory, _launcherUpdate.FileName);

            if (File.Exists(filepath))
            {
                new Process
                {
                    StartInfo =
                    {
                        FileName = filepath,
                        Arguments = "/VERYSILENT /DIR=\"" + Constants.AppDirectory + "\""
                    }
                }.Start();

                Environment.Exit(0);
            }
        }

        private static void DeleteOldUpdates()
        {
            var oldfile = Path.Combine(Constants.AppDirectory, Constants.UpdateName);

            if (File.Exists(oldfile))
            {
                var versInfo = FileVersionInfo.GetVersionInfo(oldfile);
                var productVersion = versInfo.ProductVersion;

                if (!CompareVersions(productVersion))
                {
                    try
                    {
                        File.Delete(oldfile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private static bool CompareVersions(string versiostringn)
        {
            var version = Utility.VersionToInt(Version.Parse(versiostringn));

            return Utility.VersionToInt(Assembly.GetEntryAssembly().GetName().Version) <
                   version;
        }
    }
}
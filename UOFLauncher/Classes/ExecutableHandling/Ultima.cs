using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class Ultima
    {
        public static UOModel UOUpdateObj;
        public static bool Is64Bit = Environment.Is64BitOperatingSystem;

        public static List<string> KnownRegistryKeyValueNames = KnownRegistryKeyValueNames = new List<string>
        {
            @"InstallDir",
            @"Install Dir"
        };

        public static List<string> KnownInstallationRegistryKeys = new List<string>
        {
            @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",
            @"Origin Worlds Online\Ultima Online\KR Legacy Beta",
            @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
            @"Origin Worlds Online\Ultima Online\1.0",
            @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
            @"EA GAMES\Ultima Online Samurai Empire",
            @"EA Games\Ultima Online: Mondain's Legacy",
            @"EA Games\Ultima Online Mondain's Legacy",
            @"EA GAMES\Ultima Online Samurai Empire\1.0",
            @"EA GAMES\Ultima Online Samurai Empire\1.00.0000",
            @"EA GAMES\Ultima Online: Samurai Empire\1.0",
            @"EA GAMES\Ultima Online: Samurai Empire\1.00.0000",
            @"EA Games\Ultima Online: Mondain's Legacy\1.0",
            @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
            @"Origin Worlds Online\Ultima Online Samurai Empire BETA\2d\1.0",
            @"Origin Worlds Online\Ultima Online Samurai Empire BETA\3d\1.0",
            @"Origin Worlds Online\Ultima Online Samurai Empire\2d\1.0",
            @"Origin Worlds Online\Ultima Online Samurai Empire\3d\1.0",
            @"Electronic Arts\EA Games\Ultima Online Classic"
        };

        public static string UltimaRegPath
        {
            get
            {
                return Is64Bit
                    ? @"SOFTWARE\Wow6432Node\Electronic Arts\EA Games\Ultima Online Classic"
                    : @"SOFTWARE\Electronic Arts\EA Games\Ultima Online Classic";
            }
        }

        public static async void DownloadUo()
        {
            try
            {
                var doc = await Updates.GetUpdates(Constants.UODownload);

                UOUpdateObj = (UOModel) Updates.CreateUpdate(typeof (UOModel), doc);

                UOUpdateObj.DisplayName = "UO Install";
                UOUpdateObj.StringType = "Ultima Online";
                UOUpdateObj.Description = "Ultima Online Installatin";

                UOUpdateObj.IsActive = true;

                if (doc != null && UOUpdateObj != null)
                {
                    EventController.InvokeUpdatesRetrieved();
                    var filepath = Path.Combine(Config.Instance.UOPath, UOUpdateObj.FileName);
                    UOUpdateObj.Status = AssemblyStatus.Downloading;
                    try
                    {
                        Messenger.Default.Send(
                            new MessengerHelper.ToastMessage(
                                "The Ultima Online installer has been added to the update manager queue."));
                        if (
                            await
                                UOUpdateObj.Download(filepath
                                    ))
                        {
                            InstallUO();
                        }
                    }
                    catch (Exception)
                    {;
                        UOUpdateObj.Status = AssemblyStatus.Error;
                    }
                }
                else
                {
                    UOUpdateObj.Status = AssemblyStatus.Error;
                }
            }
            catch (Exception)
            {
                UOUpdateObj.Status = AssemblyStatus.Error;
            }
        }

        private static void InstallUO()
        {
            try
            {
                UOUpdateObj.Status = AssemblyStatus.Installing;
                var uoProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(Config.Instance.UOPath, UOUpdateObj.FileName),
                        Arguments = "/DIR=\"" + Config.Instance.UOPath + "\"",
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };
                uoProcess.Exited += delegate
                {
                    if (uoProcess.ExitCode == 0)
                    {
                        if (File.Exists(Path.Combine(Config.Instance.UOPath, UOUpdateObj.FileName)))
                            File.Delete(Path.Combine(Config.Instance.UOPath, UOUpdateObj.FileName));

                        if (!IsInstalled())
                        {
                            var installs = FindInstalls();

                            if (installs.Count > 0)
                            {
                                Config.Instance.UOPath = installs[0];
                            }
                        }

                        Messenger.Default.Send(new MessengerHelper.ToastMessage("Ultima Online has finished installing."));

                        Messenger.Default.Send(
                            "CheckPlayInstall");

                        Updates.Instance.QueueLoad = true;

                        if (IsInstalled())
                            Updates.InitializeUpdates();
                      
                        UOUpdateObj = null;

                        EventController.InvokeDownloadsComplete();
                    }
                    else
                    {
                        UOUpdateObj.Status = AssemblyStatus.Error;
                        Messenger.Default.Send(
                            new MessengerHelper.ToastMessage(
                                "The Ultima Online installer could not install Ultima Online."));
                    }
                };
                uoProcess.Start();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(
                    new MessengerHelper.ToastMessage("The Ultima Online installer was unable to start."));
            }
        }

        public static void CheckPath()
        {
            var list = FindInstalls();

            if (list.Count > 1 && !list.Exists(x => x == Config.Instance.UOPath))
            {
                MessageBox.Show(
                    "Ultima Online was installed to a different folder than your currently selected Ultima Online directory. Multiple Ultima Online installs were also found, so this could not be updated automatically. Please manually update your Ultima Online directory in the settings menu.");
            }
            else if (list.Count == 1 && !list.Exists(x => x == Config.Instance.UOPath))
            {
                Config.Instance.UOPath = list[0];
            }
        }

        public static bool IsUORunning()
        {
            var pname = Process.GetProcessesByName("client");

            if (pname.Length > 0)
            {
                MessageBox.Show(
                    "An instance of the Ultima Online client was found running. Please exit out of all Ultima Online client instances before trying to update.",
                    "Update Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }

            return false;
        }

        public static bool IsUOP()
        {
            if (File.Exists(Path.Combine(Config.Instance.UOPath, "client.exe")))
            {
                var uoVersion = FileVersionInfo.GetVersionInfo(Path.Combine(Config.Instance.UOPath, "client.exe"));

                if (uoVersion.FileVersion == null)
                    return false;

                var version1 = new Version(uoVersion.FileVersion.Replace(",", "."));
                var version2 = new Version("7.0.24.0");

                if (version2.CompareTo(version1) > 0)
                    return false;

                return true;
            }

            return false;
        }

        public static bool IsInstalled()
        {
            return File.Exists(Path.Combine(Config.Instance.UOPath, "client.exe"));
        }

        public static List<string> FindInstalls()
        {
            var installs = new List<string>();
            var prefix = !Environment.Is64BitOperatingSystem ? @"SOFTWARE\" : @"SOFTWARE\Wow6432Node\";

            foreach (var knownKeyName in KnownInstallationRegistryKeys)
            {
                if (!string.IsNullOrWhiteSpace(knownKeyName))
                {
                    string path;

                    if (TryGetPath(prefix + knownKeyName, out path))
                        installs.Add(path);
                }
            }

            return installs;
        }

        private static bool TryGetPath(string regPath, out string path)
        {
            try
            {
                using (
                    var key = Registry.LocalMachine.OpenSubKey(regPath) ??
                              Registry.CurrentUser.OpenSubKey(regPath))
                {
                    if (key == null)
                    {
                        path = null;
                        return false;
                    }

                    string dir = null;

                    foreach (var knownKeyValueName in KnownRegistryKeyValueNames)
                    {
                        dir = key.GetValue(knownKeyValueName) as string;

                        if (!string.IsNullOrWhiteSpace(dir))
                        {
                            var fullPath = dir.Replace('/', '\\');

                            if (fullPath[fullPath.Length - 1] != '\\')
                                fullPath += '\\';

                            fullPath = Path.Combine(fullPath, "client.exe");

                            if (File.Exists(fullPath))
                            {
                                path = dir;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            path = null;
            return false;
        }

        public static void InitializePath()
        {
            if (string.IsNullOrEmpty(Config.Instance.UOPath) || !IsInstalled())
            {
                var installs = FindInstalls();

                if (installs.Count == 1)
                {
                    Config.Instance.UOPath = installs[0];
                }
                else if (installs.Count > 1)
                    MessageBox.Show(
                        "Multiple Installations of Ultima Online were found.  Please go to the settings section of the launcher and manually set the path to your desired Ultima Online installation.");
                else if (installs.Count == 0)
                {
                    Config.Instance.UOPath = Path.Combine(Utility.GetProgramFilesDirectory(),
                        @"Electronic Arts\Ultima Online Classic");

                    if (Directory.Exists(Path.Combine(Utility.GetProgramFilesDirectory(),
                        @"Electronic Arts\Ultima Online Classic")))
                    {
                        Directory.CreateDirectory(Path.Combine(Utility.GetProgramFilesDirectory(),
                            @"Electronic Arts\Ultima Online Classic"));
                    }
                }
            }
        }
    }
}
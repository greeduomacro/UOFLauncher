using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class Steam
    {
        private static readonly List<string> KnownExeStrings = new List<string>
        {
            @"UOS.exe",
            @"Steam.exe",
            @"UOSteam.exe"
        };

        public static bool CheckInstalled()
        {
            return KnownExeStrings.Any(exestring => File.Exists(Path.Combine(Config.Instance.SteamPath, exestring)));
        }

        public static string GetExeString()
        {
            return
                KnownExeStrings.FirstOrDefault(
                    exestring => File.Exists(Path.Combine(Config.Instance.SteamPath, exestring)));
        }

        public static void RunSteam()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = { FileName = Path.Combine(Config.Instance.SteamPath, GetExeString()), WorkingDirectory = Config.Instance.SteamPath }
                };

                process.Start();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "A valid installation of Steam could not be found. Please set the path to your Steam folder through the settings menu.");
            }
        }
    }
}
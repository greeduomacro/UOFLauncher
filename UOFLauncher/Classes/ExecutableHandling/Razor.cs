using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class Razor
    {
        public static string RazorRegPath
        {
            get { return Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\Razor" : @"SOFTWARE\Razor"; }
        }

        public static bool CheckInstalled()
        {
            return File.Exists(Path.Combine(Config.Instance.RazorPath, "Razor.exe"));
        }

        public static string FindInstall()
        {
            string install = null;

            TryGetPath(RazorRegPath, out install);

            return install;
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


                    dir = key.GetValue("InstallDir") as string;

                    if (!string.IsNullOrWhiteSpace(dir))
                    {
                        var fullPath = dir.Replace('/', '\\');

                        if (fullPath[fullPath.Length - 1] != '\\')
                            fullPath += '\\';

                        fullPath = Path.Combine(fullPath, "Razor.exe");

                        if (File.Exists(fullPath))
                        {
                            path = dir;
                            return true;
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

        public static void RunRazor()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = { FileName = Path.Combine(Config.Instance.RazorPath, "Razor.exe"), WorkingDirectory = Config.Instance.RazorPath }
                };

                process.Start();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "A valid installation of Razor could not be found. Please set the path to your Razor folder through the settings menu.");
            }
        }

        public static void InitializePath()
        {
            if (!CheckInstalled())
            {
                var install = FindInstall();

                if (!string.IsNullOrEmpty(install))
                {
                    Config.Instance.RazorPath = install;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class Sallos
    {
        private static readonly List<string> KnownExeStrings = new List<string>
        {
            @"SallosLauncher.exe",
            @"Sallos.exe"
        };

        public static bool CheckInstalled()
        {
            return KnownExeStrings.Any(exestring => File.Exists(Path.Combine(Config.Instance.SallosPath, exestring)));
        }

        public static string GetExeString()
        {
            return
                KnownExeStrings.FirstOrDefault(
                    exestring => File.Exists(Path.Combine(Config.Instance.SallosPath, exestring)));
        }

        public static void RunSallos()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = {FileName = Path.Combine(Config.Instance.SallosPath, GetExeString()), WorkingDirectory = Config.Instance.SallosPath }
                };

                process.Start();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "A valid installation of Sallos could not be found. Please set the path to your Sallos folder through the settings menu.");
            }
        }
    }
}
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using UOFLauncher.Classes;
using UOFLauncher.Models;
using UOFLauncher.Views;

namespace UOFLauncher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        protected override async void OnStartup(StartupEventArgs e)
        {
            CreateMutex(e);

            Utility.CreateFileFromResource(Constants.ConfigFilePath, Constants.ConfigResource);
            MainWindow = new MainWindow();

            var window = new LauncherUpdate_View();
            window.Show();

            await Task.Delay(2000);

            if (await Launcher.CheckForUpdate())
            {
                await Launcher.BeginUpdates();
            }
            else
            {
                window.Close();
                MainWindow.Show();
            }

            Ultima.InitializePath();

            Razor.InitializePath();

            if (Ultima.IsInstalled())
                Updates.InitializeUpdates();
             
        }

        private void CreateMutex(StartupEventArgs e)
        {
            bool createdNew;
            _mutex = new Mutex(true,
                Utility.Md5Hash(Utility.Md5Checksum(Constants.AppDirectory) +
                                Utility.Md5Hash(Environment.UserName)), out createdNew);
            if (!createdNew)
            {
                if (e.Args.Length > 0)
                {
                    var wnd = Win32Imports.FindWindow(IntPtr.Zero, "UOFLauncher");
                    if (wnd != IntPtr.Zero)
                    {
                        Clipboard.SetText(e.Args[0]);
                        ShowWindow(wnd, 5);
                        SetForegroundWindow(wnd);
                    }
                }

                _mutex = null;
                Environment.Exit(0);
            }
        }
    }
}
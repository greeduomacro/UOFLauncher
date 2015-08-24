using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Models;

namespace UOFLauncher.ViewModels
{
    public class LauncherUpdate_ViewModel : MyBase_ViewModel
    {
        private int _downloadProgress;
        private int _maxDownload;
        private int _currentlyDownloaded;
        private bool _isIndeterminate;
        private string _updateMessage;

        public LauncherUpdate_ViewModel()
        {
            IsIndeterminate = true;
            UpdateMessage = "Starting Ultima Online: Forever Launcher.";

            Messenger.Default.Register<ProgressObject>(this, obj =>
            {
                if (IsIndeterminate)
                {
                    UpdateMessage = "Updating Ultima Online: Forever Launcher.";
                    IsIndeterminate = false;
                }

                _maxDownload += obj.TotalDownload;
                _currentlyDownloaded += obj.Downloaded;

                DownloadProgress = (int)Math.Round(((double)_currentlyDownloaded / _maxDownload) * 100);
            });
        }

        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set
            {
                _downloadProgress = value;
                RaisePropertyChanged();
            }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                _isIndeterminate = value;
                RaisePropertyChanged();
            }
        }

        public string UpdateMessage
        {
            get { return _updateMessage; }
            set
            {
                _updateMessage = value;
                RaisePropertyChanged();
            }
        }
    }
}

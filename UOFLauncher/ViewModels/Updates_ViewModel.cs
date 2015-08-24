using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Classes;
using UOFLauncher.Models;

namespace UOFLauncher.ViewModels
{
    public class Updates_ViewModel : MyBase_ViewModel
    {
        private ICommand _LoadCommand;
        private ICommand _UpdateAllCommand;
        private ICommand _UpdateSelectedCommand;

        public Updates_ViewModel()
        {
            Messenger.Default.Register<ProgressObject>(this, obj =>
            {
                _maxDownload += obj.TotalDownload;
                _currentlyDownloaded += obj.Downloaded;

                DownloadProgress = (int)Math.Round(((double)_currentlyDownloaded / _maxDownload) * 100);
            });
        }

        private int _downloadProgress;

        private int _maxDownload;

        private int _currentlyDownloaded;

        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set
            {
                _downloadProgress = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<UpdateObject> UpdatesCollection
        {
            get { return Updates.Instance.UpdatesCollection; }
        }

        public ICommand Load_Command
        {
            get
            {
                return _LoadCommand ??
                       (_LoadCommand = new RelayCommand<object>(Execute_OnLoaded));
            }
        }

        public ICommand UpdateAll_Command
        {
            get
            {
                return _UpdateAllCommand ??
                       (_UpdateAllCommand = new RelayCommand<object>(Execute_UpdateAll));
            }
        }

        public ICommand UpdateSelected_Command
        {
            get
            {
                return _UpdateSelectedCommand ??
                       (_UpdateSelectedCommand = new RelayCommand<object>(Execute_UpdateSelected));
            }
        }

        private void Execute_OnLoaded(object loadObj)
        {
            if (Updates.Instance.QueueLoad)
            {
                Updates.Instance.QueueLoad = false;
                var autoupdate = Config.GetSetting("UpdateOnStart");

                var doupdate = autoupdate == "True";

                //Downloader.GetUpdates(UpdateType.UO, doupdate);
            }
        }

        private void Execute_UpdateAll(object anObject)
        {
            Updates.Update_All();
        }

        private void Execute_UpdateSelected(object anObject)
        {
            Updates.Update_Selected();
        }
    }
}
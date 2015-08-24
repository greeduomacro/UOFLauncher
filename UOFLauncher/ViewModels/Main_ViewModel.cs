using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Classes;
using UOFLauncher.Models;

namespace UOFLauncher.ViewModels
{
    public sealed class Main_ViewModel : MyBase_ViewModel
    {
        private int _downloadProgress;
        private int _maxDownload;
        private int _currentlyDownloaded;
        private ICommand _onCloseCommand;
        private ICommand _onLoadCommand;
        private string _showDownloads;
        private string _toastMessage;

        public Main_ViewModel()
        {
            Messenger.Default.Register<MessengerHelper.ToastMessage>(this, msg => ToastMessage = msg.Message);

            EventController.DownloadsCompleted += Event_DownloadsCompleted;
            EventController.UpdatesRetrieved += Event_UpdatesRetrieved;

            Messenger.Default.Register<ProgressObject>(this, obj =>
            {
                _maxDownload += obj.TotalDownload;
                _currentlyDownloaded += obj.Downloaded;

                DownloadProgress = (int)Math.Round(((double)_currentlyDownloaded / _maxDownload) * 100);
            });

            ShowDownloads = "Collapsed";
        }

        public string ToastMessage
        {
            get { return _toastMessage; }
            set
            {
                _toastMessage = value;
                RaisePropertyChanged();
            }
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

        public string ShowDownloads
        {
            get { return _showDownloads; }
            set
            {
                _showDownloads = value;
                RaisePropertyChanged();
            }
        }

        public ICommand OnCloseCommand
        {
            get { return _onCloseCommand ?? (_onCloseCommand = new RelayCommand<CancelEventArgs>(Execute_Close)); }
        }

        public ICommand Load_Command
        {
            get
            {
                return _onLoadCommand ??
                       (_onLoadCommand = new RelayCommand<object>(Execute_OnLoaded));
            }
        }

        public void ReceiveMessage(ObservableCollection<BaseUpdateObject> collection)
        {
            if (collection.Count > 0 && ShowDownloads == "Collapsed")
            {
                ShowDownloads = "Visible";
            }
        }

        private void Execute_Close(CancelEventArgs e)
        {
            var isDownloading =
                Updates.Instance.UpdatesCollection.Any(
                    update => update.IsActive) || Ultima.UOUpdateObj != null && Ultima.UOUpdateObj.IsActive;

            if (isDownloading)
            {
                var result =
                    MessageBox.Show(
                        "There are currently important updates downloading. If you exit now, the updates will not be installed. \n\nYou may run the launcher at another time to resume the downloads.\n\nExit the Launcher?",
                        "Exit Launcher", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Execute_OnLoaded(object loadObj)
        {
            /*if (Updates.Instance.QueueLoad)
            {
                Updates.Instance.QueueLoad = false;
                var autoupdate = Config.GetSetting("UpdateOnStart");

                var doupdate = autoupdate == "True";

                Updater.GetUpdates(UpdateType.UO, doupdate);
            }*/
        }

        private void Event_UpdatesRetrieved()
        {
            ShowDownloads = "Visible"; 
        }

        private void Event_DownloadsCompleted()
        {
            _maxDownload = 0;
            _currentlyDownloaded = 0;
            _downloadProgress = 0;

            if (Updates.Instance.UpdatesCollection.Count == 0)
                ShowDownloads = "Collapsed";           
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UOFLauncher.Classes;
using UOFLauncher.Models;

namespace UOFLauncher.ViewModels
{
    public class Settings_ViewModel : MyBase_ViewModel
    {
        private ICommand _LoadedCommand;
        private ICommand _RazorFolderDialogCommand;
        private ICommand _SallosFolderDialogCommand;
        private ICommand _SelectionCommand;
        private object _settingsFrame;
        private ICommand _SteamFolderDialogCommand;
        private ICommand _UOFolderDialogCommand;
        public bool IsInit { get; set; }

        public Config Config
        {
            get { return Config.Instance; }
        }

        public ICommand Loaded_Command
        {
            get
            {
                return _LoadedCommand ??
                       (_LoadedCommand = new RelayCommand<object>(Execute_Loaded));
            }
        }

        public ICommand SelectionCommand
        {
            get
            {
                return _SelectionCommand ??
                       (_SelectionCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(Execute_Selected));
            }
        }

        public object SettingsFrame
        {
            get { return _settingsFrame; }
            set
            {
                _settingsFrame = value;
                RaisePropertyChanged();
            }
        }

        public ICommand UOFolderDialogCommand
        {
            get
            {
                return _UOFolderDialogCommand ??
                       (_UOFolderDialogCommand = new RelayCommand<object>(Execute_UOFolderDialog));
            }
        }

        public ICommand RazorFolderDialogCommand
        {
            get
            {
                return _RazorFolderDialogCommand ??
                       (_RazorFolderDialogCommand = new RelayCommand<object>(Execute_RazorFolderDialog));
            }
        }

        public ICommand SteamFolderDialogCommand
        {
            get
            {
                return _SteamFolderDialogCommand ??
                       (_SteamFolderDialogCommand = new RelayCommand<object>(Execute_SteamFolderDialog));
            }
        }

        public ICommand SallosFolderDialogCommand
        {
            get
            {
                return _SallosFolderDialogCommand ??
                       (_SallosFolderDialogCommand = new RelayCommand<object>(Execute_SallosFolderDialog));
            }
        }

        private void Execute_Loaded(object button)
        {
            if (!IsInit)
            {
                IsInit = true;
                SettingsFrame =
                    Activator.CreateInstance(null, "UOFLauncher.Views.SpecificSettingsViews.GeneralSettings").Unwrap();
            }
        }

        private void Execute_Selected(RoutedPropertyChangedEventArgs<object> e)
        {
            var name = ((TreeViewItem) ((TreeView) e.Source).SelectedItem).Uid;
            SettingsFrame = Activator.CreateInstance(null, "UOFLauncher.Views.SpecificSettingsViews." + name).Unwrap();
        }

        private void Execute_UOFolderDialog(object buttone)
        {
            var folderDialog = new FolderSelectDialog();

            folderDialog.Title = "Select Ultima Online Folder";

            folderDialog.InitialDirectory = Config.Instance.UOPath;

            if (folderDialog.ShowDialog())
            {
                Config.UOPath = folderDialog.FileName;
            }
        }

        private void Execute_RazorFolderDialog(object buttone)
        {
            var folderDialog = new FolderSelectDialog();

            folderDialog.Title = "Select Razor Folder";

            folderDialog.InitialDirectory = Config.Instance.RazorPath;

            if (folderDialog.ShowDialog())
            {
                Config.RazorPath = folderDialog.FileName;
            }
        }

        private void Execute_SteamFolderDialog(object buttone)
        {
            var folderDialog = new FolderSelectDialog();

            folderDialog.Title = "Select Steam Folder";

            folderDialog.InitialDirectory = Config.Instance.SteamPath;

            if (folderDialog.ShowDialog())
            {
                Config.SteamPath = folderDialog.FileName;
            }
        }

        private void Execute_SallosFolderDialog(object buttone)
        {
            var folderDialog = new FolderSelectDialog();

            folderDialog.Title = "Select Sallos Folder";

            folderDialog.InitialDirectory = Config.Instance.SallosPath;

            if (folderDialog.ShowDialog())
            {
                Config.SallosPath = folderDialog.FileName;
            }
        }
    }
}
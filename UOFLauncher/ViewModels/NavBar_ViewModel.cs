using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Classes;
using UOFLauncher.Models;

namespace UOFLauncher.ViewModels
{
    public class NavBar_ViewModel : MyBase_ViewModel
    {
        private string _showUoInstall;
        private string _ShowUOPlay;

        public NavBar_ViewModel()
        {
            Messenger.Default.Register<string>(this, obj =>
            {
                if (obj == "CheckPlayInstall")
                {
                    Event_CheckInstallPlay();
                }
            });

            if (Ultima.IsInstalled())
            {
                ShowUOPlay = "Visible";
                ShowUoInstall = "Collapsed";
            }
            else
            {
                ShowUOPlay = "Collapsed";
                ShowUoInstall = "Visible";
            }
        }

        public string ShowUOPlay
        {
            get { return _ShowUOPlay; }
            set
            {
                _ShowUOPlay = value;
                RaisePropertyChanged();
            }
        }

        public string ShowUoInstall
        {
            get { return _showUoInstall; }
            set
            {
                _showUoInstall = value;
                RaisePropertyChanged();
            }
        }

        public void Event_CheckInstallPlay()
        {
            if (Ultima.IsInstalled())
            {
                ShowUOPlay = "Visible";
                ShowUoInstall = "Collapsed";
            }
            else
            {
                ShowUOPlay = "Collapsed";
                ShowUoInstall = "Visible";
            }
        }
    }
}
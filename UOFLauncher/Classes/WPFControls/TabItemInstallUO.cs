using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class TabItemInstallUO : TabItem
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Ultima.UOUpdateObj != null && Ultima.UOUpdateObj.IsActive)
            {
                Messenger.Default.Send(
                    new MessengerHelper.ToastMessage("Ultima Online is already being downloaded and installed."));
            }
            else
            {
                if (!Ultima.IsInstalled() && string.IsNullOrEmpty(Config.Instance.UOPath))
                {
                    var folderDialog = new FolderSelectDialog();

                    folderDialog.Title = "Select Ultima Online Folder";

                    if (folderDialog.ShowDialog())
                    {
                        Config.Instance.UOPath = folderDialog.FileName;
                        Ultima.DownloadUo();
                    }
                }
                else if (!Ultima.IsInstalled())
                {
                    Ultima.DownloadUo();
                }
            }
        }
    }
}
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Models;

namespace UOFLauncher.Classes
{
    public class TabItemPlay : TabItem
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!Ultima.IsInstalled())
            {
                MessageBox.Show(
                    "A valid installation of Ultima Online was not found. Please go to the settings section of the launcher and manually set the path to your Ultima Online install folder.",
                    "No Ultima Online Installation Found",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (
                Updates.Instance.UpdatesCollection.Any(
                    update => update.Status == AssemblyStatus.Downloading || update.Status == AssemblyStatus.Installing))
            {
                Messenger.Default.Send(
                    new MessengerHelper.ToastMessage(
                        "Ultima Online could not be started as there are updates downloading or installing in the updates manager."));
                return;
            }

            var assitant = Config.GetSetting("SelectedAssistant");
            if (!string.IsNullOrEmpty(assitant))
            {
                switch (assitant)
                {
                    case "Razor":
                    {
                        Razor.RunRazor();
                        break;
                    }
                    case "Steam":
                    {
                        Steam.RunSteam();
                        break;
                    }
                    case "Sallos":
                    {
                        Sallos.RunSallos();
                        break;
                    }
                }
            }
        }
    }
}
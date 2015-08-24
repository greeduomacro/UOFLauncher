using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UOFLauncher.Classes
{
    public class TabItemExit : TabItem
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
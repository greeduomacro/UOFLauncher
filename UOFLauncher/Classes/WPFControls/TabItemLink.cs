using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace UOFLauncher.Classes
{
    public class TabItemLink : TabItem
    {
        public string Link { get; set; }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            Process.Start(Link);
        }
    }
}
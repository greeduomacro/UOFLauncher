using System.Windows;
using System.Windows.Controls;

namespace UOFLauncher.Views
{
    /// <summary>
    ///     Interaction logic for TabItemHeader.xaml
    /// </summary>
    public partial class TabItemHeader_View : UserControl
    {
        public static DependencyProperty IMGWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof (int), typeof (TabItemHeader_View));

        public static DependencyProperty IMGHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof (int), typeof (TabItemHeader_View));

        public static DependencyProperty IMGSourceProperty =
            DependencyProperty.Register("ImageSource", typeof (Canvas), typeof (TabItemHeader_View));

        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (TabItemHeader_View));

        public TabItemHeader_View()
        {
            InitializeComponent();
        }

        public Canvas ImageSource
        {
            get { return (Canvas) GetValue(IMGSourceProperty); }
            set { SetValue(IMGSourceProperty, value); }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int ImageWidth
        {
            get { return (int) GetValue(IMGSourceProperty); }
            set { SetValue(IMGSourceProperty, value); }
        }

        public int ImageHeight
        {
            get { return (int) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
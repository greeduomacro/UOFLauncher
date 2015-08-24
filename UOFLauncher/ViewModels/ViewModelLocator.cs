using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace UOFLauncher.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);


            SimpleIoc.Default.Register<Main_ViewModel>();
            SimpleIoc.Default.Register<NavBar_ViewModel>();
            SimpleIoc.Default.Register<Updates_ViewModel>();
            SimpleIoc.Default.Register<Settings_ViewModel>();
            SimpleIoc.Default.Register<LauncherUpdate_ViewModel>();
        }

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public Main_ViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<Main_ViewModel>(); }
        }

        public NavBar_ViewModel NavBar_VM
        {
            get { return ServiceLocator.Current.GetInstance<NavBar_ViewModel>(); }
        }

        public TabItemHeader_ViewModel TabItemHeader_VM
        {
            get { return ServiceLocator.Current.GetInstance<TabItemHeader_ViewModel>(); }
        }

        public Updates_ViewModel Updates_VM
        {
            get { return ServiceLocator.Current.GetInstance<Updates_ViewModel>(); }
        }

        public Settings_ViewModel Settings_VM
        {
            get { return ServiceLocator.Current.GetInstance<Settings_ViewModel>(); }
        }

        public LauncherUpdate_ViewModel LauncherUpdate_VM
        {
            get { return ServiceLocator.Current.GetInstance<LauncherUpdate_ViewModel>(); }
        }

        public static void Cleanup()
        {
        }
    }
}
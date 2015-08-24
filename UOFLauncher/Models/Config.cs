#region LICENSE

// Copyright 2014 LeagueSharp.Loader
// LeagueSharpAssembly.cs is part of LeagueSharp.Loader.
// 
// LeagueSharp.Loader is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Loader is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Loader. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Classes;

namespace UOFLauncher.Models
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Config : ViewModelBase
    {
        private string _RazorPath;
        private string _SallosPath;
        private ConfigSettings _settings;
        private string _SteamPath;
        private string _UOPath;

        [XmlIgnore]
        public static Config Instance { get; set; }

        [XmlIgnore]
        public bool Initiated { get; set; }

        public string UOPath
        {
            get { return _UOPath; }
            set
            {
                _UOPath = value;
                RaisePropertyChanged();

                if (Initiated)
                {
                    Updates.Instance.QueueLoad = true;
                        Messenger.Default.Send(
                            "CheckPlayInstall");
                    ConfigChanged();
                }
            }
        }

        public string RazorPath
        {
            get { return _RazorPath; }
            set
            {
                _RazorPath = value;
                RaisePropertyChanged();

                if (Initiated)
                {
                    Updates.Instance.QueueLoad = true;
                    ConfigChanged();
                }
            }
        }

        public string SallosPath
        {
            get { return _SallosPath; }
            set
            {
                _SallosPath = value;
                RaisePropertyChanged();

                if (Initiated)
                {
                    Updates.Instance.QueueLoad = true;
                    ConfigChanged();
                }
            }
        }

        public string SteamPath
        {
            get { return _SteamPath; }
            set
            {
                _SteamPath = value;
                RaisePropertyChanged();

                if (Initiated)
                {
                    Updates.Instance.QueueLoad = true;
                    ConfigChanged();
                }
            }
        }

        public ConfigSettings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                RaisePropertyChanged();

                if (Initiated)
                    ConfigChanged();
            }
        }

        public static void ConfigChanged()
        {
            try
            {
                Utility.MapClassToXmlFile(typeof (Config), Instance, Constants.ConfigFilePath);
            }
            catch
            {
                MessageBox.Show("Error writing to configuration file.");
            }
        }

        public static string GetSetting(string settingstring)
        {
            if (Instance.Settings != null)
            {
                return
                    (from setting in Instance.Settings.GeneralSettings
                        where setting.Name == settingstring
                        select setting.SelectedValue).FirstOrDefault();
            }

            return null;
        }
    }

    [XmlType(AnonymousType = true)]
    public class ConfigSettings : ViewModelBase
    {
        private ObservableCollection<GeneralSettings> _generalSettings;

        [XmlArrayItem("GeneralSettings", IsNullable = true)]
        public ObservableCollection<GeneralSettings> GeneralSettings
        {
            get { return _generalSettings; }
            set
            {
                _generalSettings = value;
                RaisePropertyChanged();
            }
        }
    }

    public class GeneralSettings : ViewModelBase
    {
        private string _displayName;
        private string _name;
        private List<string> _possibleValues;
        private string _selectedValue;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                RaisePropertyChanged();
            }
        }

        public List<string> PossibleValues
        {
            get { return _possibleValues; }
            set
            {
                _possibleValues = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                _selectedValue = value;
                RaisePropertyChanged();

                if (Config.Instance != null && Config.Instance.Initiated)
                    Config.ConfigChanged();
            }
        }
    }
}
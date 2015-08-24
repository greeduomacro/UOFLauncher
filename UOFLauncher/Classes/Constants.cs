#region LICENSE

// Copyright 2014 LeagueSharp.Loader
// Directories.cs is part of LeagueSharp.Loader.
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

using System;
using System.IO;

namespace UOFLauncher.Classes
{
    public static class Constants
    {
        public static readonly string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string LogsDir = Path.Combine(AppDirectory, "Logs") + "\\";
        public static readonly string ConfigFilePath = Path.Combine(AppDirectory, "config.xml");

        public static readonly string ConfigResource = "UOFLauncher.Resources.config.xml";

        public static readonly string UpdateName = "Launcher-update.exe";

        public static readonly string LauncherDownload = "http://www.uoforever.com/patches/Launcher/Update.xml";
        public static readonly string UODownload = "http://www.uoforever.com/patches/UO/Update.xml";
        public static readonly string UOPDownload = "http://www.uoforever.com/patches/UOP/Updates.xml";
        public static readonly string MULDownload = "http://www.uoforever.com/patches/MUL/Updates.xml";
    }
}
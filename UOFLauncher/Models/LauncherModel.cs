using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UOFLauncher.Models
{
    [Serializable]
    [XmlRoot("LauncherUpdate")]
    public class LauncherUdateObj : BaseUpdateObject 
    {
        [XmlElement("Version")]
        public string Version { get; set; }
    }
}

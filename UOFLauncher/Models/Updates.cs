using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Classes;

namespace UOFLauncher.Models
{
    public enum UpdateType
    {
        UOInstall,
        UO,
        Launcher
    }

    public class ProgressObject
    {
        public ProgressObject(int downloaded, int totaldownload)
        {
            Downloaded = downloaded;
            TotalDownload = totaldownload;
        }

        public int Downloaded { get; set; }
        public int TotalDownload { get; set; }
    }

    [Serializable]
    [XmlRoot("Updates")]
    public class Updates : ViewModelBase
    {
        public static IProgress<ProgressObject> Progress = new Progress<ProgressObject>(ReportProgress);
        private bool _queueLoad;
        private ObservableCollection<UpdateObject> _UpdatesCollection;

        public Updates()
        {
            UpdatesCollection = new ObservableCollection<UpdateObject>();
            Messenger.Default.Register<UpdateObject>
                (
                    this,
                    RemoveFromCollection
                );
        }

        public static Updates Instance = new Updates();

        [XmlArray("UpdateCollection")]
        [XmlArrayItem("UpdateObject", typeof (UpdateObject))]
        public ObservableCollection<UpdateObject> UpdatesCollection
        {
            get { return _UpdatesCollection; }
            set
            {
                _UpdatesCollection = value;
                RaisePropertyChanged();
            }
        }

        public bool QueueLoad
        {
            get { return _queueLoad; }
            set
            {
                _queueLoad = value;
                RaisePropertyChanged();
            }
        }

        public static void ReportProgress(ProgressObject progressobject)
        {
            Messenger.Default.Send(
                progressobject);
        }

        public bool Exists(string name)
        {
            return UpdatesCollection.ToArray().Any(update => update.FileName == name);
        }

        public void RemoveFromCollection(UpdateObject obj)
        {
            UpdatesCollection.Remove(obj);

            if (UpdatesCollection.Count == 0 || UpdatesCollection.All(x => !x.IsActive))
                EventController.InvokeDownloadsComplete();
        }

        public static async void InitializeUpdates()
        {
            var url = Ultima.IsUOP() ? Constants.UOPDownload : Constants.MULDownload;

            var doc = await GetUpdates(url);

            if (doc != null)
                Instance = ((Updates)Utility.MapXmlDocToClass(typeof(Updates), doc));

            foreach (var update in Instance.UpdatesCollection.ToArray())
            {
                string path = Path.Combine(update.Location, update.DisplayName);

                if (File.Exists(path))
                {
                    var localhash = Utility.Md5Checksum(path);
                    if (localhash == update.Hash.ToLower())
                    {
                        Instance.UpdatesCollection.Remove(update);
                    }
                    else
                    {
                        update.Status = AssemblyStatus.Outdated;
                    }
                }
                else
                {
                    update.Status = AssemblyStatus.Outdated;
                }
            }

            if (Instance.UpdatesCollection.Count > 0)
            {       
                EventController.InvokeUpdatesRetrieved();

                if (Config.GetSetting("UpdateOnStart") == "True")
                {
                    Update_All();
                }
            }
        }

        public static async Task<XDocument> GetUpdates(string url)
        {
            try
            {
                var wb = new LauncherWebClient {Proxy = null};

                var data = await wb.DownloadStringTaskAsync(
                    new Uri(url));

                try
                {
                    return XDocument.Parse(data);
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static void Update_All()
        {
            if (!Ultima.IsUORunning())
            {
                foreach (var updateobj in Instance.UpdatesCollection)
                {
                    if (!updateobj.IsActive)
                        Update(updateobj);
                }
            }
        }

        public static void Update_Selected()
        {
            if (!Ultima.IsUORunning())
            {
                foreach (
                    var updateobj in Instance.UpdatesCollection.Where(updateobj => updateobj.SelectedForUpdate))
                {
                    if (!updateobj.IsActive)
                        Update(updateobj);
                }
            }
        }

        public static BaseUpdateObject CreateUpdate(Type type, XDocument doc)
        {
            return (BaseUpdateObject) Utility.MapXmlDocToClass(type, doc);
        }

        public static async void Update(UpdateObject obj)
        {
            var path = Path.Combine(Config.Instance.UOPath, obj.FileName);

            if (!Directory.Exists(Config.Instance.UOPath))
                Directory.CreateDirectory(Config.Instance.UOPath);

            obj.Status = AssemblyStatus.Downloading;

            if (await obj.Download(path))
            {
                obj.ExecuteFile();
            }

            if (obj.Status == AssemblyStatus.Finished)
            {
                Instance.RemoveFromCollection(obj);
            }
        }
    }
}
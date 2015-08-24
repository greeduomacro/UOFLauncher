using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using UOFLauncher.Classes;

namespace UOFLauncher.Models
{
    [Serializable]
    public abstract class BaseUpdateObject :ViewModelBase
    {
        private string _fileName;
        private AssemblyStatus _status;
        private string _displayStatus;
        private string _description;
        private string _displayName;
        private string _downloadPercent;
        private int _totalDownloaded;

        private bool _selectedForUpdate;
        private string _stringType;

        public bool IsActive { get; set; }

        [XmlElement("Hash")]
        public string Hash { get; set; }

        [XmlIgnore]
        public string Location { get; set; }

        [XmlIgnore]
        public int MaxDownload { get; set; }

        [XmlIgnore]
        public int TotalDownloaded
        {
            get { return _totalDownloaded; }
            set
            {
                _totalDownloaded = value;
                ComputePercent();
            }
        }

        [XmlIgnore]
        public bool SelectedForUpdate
        {
            get { return _selectedForUpdate; }
            set
            {
                _selectedForUpdate = value;
                RaisePropertyChanged();
            }
        }

        [XmlElement("StringType")]
        public string StringType
        {
            get { return _stringType; }
            set
            {
                switch (value)
                {
                    case "Ultima Online":
                        Location = Config.Instance.UOPath;
                        break;
                }
                _stringType = value;
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string DownloadPercent
        {
            get { return _downloadPercent; }
            set
            {
                _downloadPercent = value;
                RaisePropertyChanged();
            }
        }

        [XmlElement("FileName")]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
            }
        }

        [XmlElement("DisplayName")]
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                RaisePropertyChanged();
            }
        }

        [XmlElement("URL")]
        public string URL { get; set; }

        public AssemblyStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged();

                CheckIsActive();
            }
        }

        public string DisplayStatus
        {
            get { return _displayStatus; }
            set
            {
                _displayStatus = value;
                RaisePropertyChanged();
            }
        }

        [XmlElement("Description")]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
        }

        public void ComputePercent()
        {
            var perecent = (int)Math.Round(((double)TotalDownloaded / MaxDownload) * 100);

            if (perecent == 0)
            {
                DownloadPercent = "";
            }
            else
                DownloadPercent = perecent + "%";

        }

        public void CheckIsActive()
        {
            switch (Status)
            {
                case AssemblyStatus.Downloading:
                {
                    IsActive = true;
                    break;
                }
                case AssemblyStatus.Installing:
                {
                    IsActive = true;
                    break;
                }
                case AssemblyStatus.Finished:
                {
                    IsActive = false;
                    break;
                }
                case AssemblyStatus.Outdated:
                {
                    IsActive = false;
                    break;
                }
                case AssemblyStatus.Error:
                {
                    IsActive = false;
                    break;
                }
            }
        }

        public async Task<bool> Download(string filepath)
        {
            var readRequest = new HttpRequestMessage(HttpMethod.Get, URL);

            var directory = Path.GetDirectoryName(filepath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var handler = new HttpClientHandler
            {
                Proxy = null,
                UseProxy = false
            };

            var client = new HttpClient(handler);

            try
            {
                using (Stream writeStream = File.Open(filepath, FileMode.Append))
                {
                    writeStream.Seek(0, SeekOrigin.End);
                    var currentLength = writeStream.Length;
                    if (currentLength > 0)
                        readRequest.Headers.Add("Range", string.Format("bytes={0}-{1}", currentLength, long.MaxValue));
                    using (var response =
                        await
                            client.SendAsync(readRequest, HttpCompletionOption.ResponseHeadersRead)
                                .ConfigureAwait(false))
                    {
                        var remoteSize = response.Content.Headers.ContentLength;
                        var totalRead = 0L;
                        if (remoteSize != 0 && remoteSize != null)
                        {
                            Updates.Progress.Report(new ProgressObject(0, (int)remoteSize));
                            MaxDownload = (int) remoteSize;
                            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                var buffer = new byte[1024 * 64];
                                int bytesRead;
                                do
                                {
                                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                                    writeStream.Write(buffer, 0, bytesRead);
                                    await writeStream.FlushAsync();
                                    totalRead += bytesRead;
                                    Updates.Progress.Report(new ProgressObject(bytesRead, 0));
                                    TotalDownloaded += bytesRead;
                                } while (bytesRead != 0);
                            }

                            return true;
                        }
                        if (remoteSize == 0 || remoteSize == null)
                            return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public void Decompress()
        {
            try
            {
                Status = AssemblyStatus.Installing;
                using (var archive = ZipFile.OpenRead(Path.Combine(Location, FileName)))
                {
                    foreach (var entry in archive.Entries)
                    {
                        try
                        {
                            var fullPath = Path.Combine(Location, entry.FullName);
                            if (string.IsNullOrEmpty(entry.Name))
                            {
                                Directory.CreateDirectory(fullPath);
                            }
                            else
                            {
                                File.Delete(Path.Combine(Location, entry.FullName));
                                entry.ExtractToFile(Path.Combine(Config.Instance.UOPath, entry.FullName), true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Unable to decompress " + entry.Name + ".");
                        }
                    }
                }

                if (File.Exists(Path.Combine(Location, FileName)))
                    File.Delete(Path.Combine(Location, FileName));

                Status = AssemblyStatus.Finished;
                Messenger.Default.Send(
                    new MessengerHelper.ToastMessage("The file " + DisplayName + " has finished decompressing."));
            }
            catch (Exception)
            {
                Status = AssemblyStatus.Error;
                MessageBox.Show("Unable to decompress " + FileName + ".");
            }
        }

        public void ExecuteFile()
        {
            if (Path.GetExtension(Path.Combine(Location, FileName)) == ".zip")
            {
                Decompress();
            }
            else if (Path.GetExtension(Path.Combine(Location, FileName)) == ".exe")
            {
                //EventController.InvokeFileExecute(new UpdateObjectEventArgs(obj));
            }
        }
    }
}

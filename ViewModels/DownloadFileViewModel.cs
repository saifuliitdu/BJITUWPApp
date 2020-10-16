using BJITUWPApp.Commands;
using BJITUWPApp.Models;
using BJITUWPApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BJITUWPApp.ViewModels
{
    class DownloadFileViewModel : INotifyPropertyChanged
    {
        DownloadOperation downloadOperation;
        CancellationTokenSource cancellationToken;
        BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        StorageFolder _folder;

        #region INotifyPropertyChanged_Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public DownloadFileViewModel() {
            DownloadCmd = new DownloadCommand(Download);
            CancelCmd = new DownloadCommand(Cancel);
            OpenCmd = new DownloadCommand(Open);
        }
        public DownloadFileViewModel(StorageFolder folder)
        {
            //_downloadFileService = new DownloadFileService();
            DownloadCmd = new DownloadCommand(Download);
            CancelCmd = new DownloadCommand(Cancel);
            OpenCmd = new DownloadCommand(Open);
            _folder = folder;
        }
        private StorageFile downloadedFile;

        public StorageFile DownloadedFile
        {
            get { return downloadedFile; }
            set { downloadedFile = value; OnPropertyChanged("DownloadedFile"); }
        }

        private string localFilePath;

        public string LocalFilePath
        {
            get { return localFilePath; }
            set { localFilePath = value; OnPropertyChanged("LocalFilePath"); }
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged("FileName"); }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; OnPropertyChanged("Url"); }
        }
        private int currentProgress;

        public int CurrentProgress
        {
            get { return currentProgress; }
            set
            {
                currentProgress = value;
                OnPropertyChanged("CurrentProgress");
            }
        }

        private string _ButtonText;
        public string ButtonText
        {
            get { return _ButtonText ?? (_ButtonText = "Download"); }
            set
            {
                _ButtonText = value;
                OnPropertyChanged("ButtonText");
            }
        }

        private ICommand _ButtonClickCommand;
        public ICommand ButtonClickCommand
        {
            get { return _ButtonClickCommand ?? (_ButtonClickCommand = DownloadCmd); }
            set
            {
                _ButtonClickCommand = value;
                OnPropertyChanged("ButtonClickCommand");
            }
        }
       
        public ICommand DownloadCmd
        {
            get;
            private set;
        }
        public ICommand OpenCmd
        {
            get;
            private set;
        }
        public ICommand CancelCmd
        {
            get;
            private set;
        }
        
        private async void Download(string url)
        {
            ButtonText = "Cancel";
            ButtonClickCommand = CancelCmd;

            if(_folder == null)
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                folderPicker.ViewMode = PickerViewMode.Thumbnail;
                folderPicker.FileTypeFilter.Add("*");
                _folder = await folderPicker.PickSingleFolderAsync();
            }
            var fileName = Helper.GetFileName(url);
            
            if (_folder != null)
            {
                StorageFile file = await _folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                downloadOperation = backgroundDownloader.CreateDownload(new Uri(url), file);
                Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);
                cancellationToken = new CancellationTokenSource();
              
                try
                {
                    await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
                }
                catch (TaskCanceledException)
                {
                    downloadOperation = null;
                }
            }
        }
        private void progressChanged(DownloadOperation downloadOperation)
        {
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
            CurrentProgress = progress;

            if (progress >= 100)
            {
                downloadedFile = (StorageFile)downloadOperation.ResultFile;
                localFilePath = downloadedFile.Path;
                ButtonText = "Open";
                ButtonClickCommand = OpenCmd;
                downloadOperation = null;
            }

        }

        private async void Open(string url)
        {
            await Windows.System.Launcher.LaunchFileAsync(downloadedFile);
        }
        private void Cancel(string url)
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();
            CurrentProgress = 0;

            // Now switch the button   
            ButtonText = "Download";
            ButtonClickCommand = DownloadCmd;
        }


    }
}

using BJITUWPApp.Commands;
using BJITUWPApp.Models;
using BJITUWPApp.Services;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace BJITUWPApp.ViewModels
{
    class DownloadListViewModel : INotifyPropertyChanged
    {
        DownloadFileService _downloadFileService;
        CancellationTokenSource cancellationToken;
        BackgroundDownloader backgroundDownloader = new BackgroundDownloader();

        #region INotifyPropertyChanged_Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public DownloadListViewModel()
        {
            _downloadFileService = new DownloadFileService();
            DownloadAllCmd = new DownloadCommand(DownloadAll);
            CancelAllCmd = new DownloadCommand(CancelAll);
            LoadFiles();
        }
        private string _MessageText;
        public string MessageText
        {
            get { return _MessageText; }
            set
            {
                _MessageText = value;
                OnPropertyChanged("MessageText");
            }
        }
        private string _ButtonText;
        public string ButtonText
        {
            get { return _ButtonText ?? (_ButtonText = "DownloadAll"); }
            set
            {
                _ButtonText = value;
                OnPropertyChanged("ButtonText");
            }
        }

        private ICommand _ButtonClickCommand;
        public ICommand ButtonClickCommand
        {
            get { return _ButtonClickCommand ?? (_ButtonClickCommand = DownloadAllCmd); }
            set
            {
                _ButtonClickCommand = value;
                OnPropertyChanged("ButtonClickCommand");
            }
        }

        public string Urls
        {
            get;
            private set;
        }
        public ICommand DownloadAllCmd
        {
            get;
            private set;
        }
        public ICommand CancelAllCmd
        {
            get;
            private set;
        }
        public ICommand OpenAllCmd
        {
            get;
            private set;
        }
        #region DisplayOperation
        private List<DownloadFileViewModel> downloadList;
        public List<DownloadFileViewModel> DownloadList
        {
            get { return downloadList; }
            set { downloadList = value; OnPropertyChanged("DownloadList"); }
        }
        private void LoadFiles()
        {
            var downloadFiles = _downloadFileService.GetFiles().Select(x => new DownloadFileViewModel { FileName = x.FileName, Url = x.Url });
            DownloadList = new List<DownloadFileViewModel>(downloadFiles);
            Urls = string.Join(",", DownloadList.Select(x => x.Url));
        }
        #endregion

        private async void DownloadAll(string urls)
        {
            ButtonText = "Downloading";
            List<StorageFile> downloadedFiles = new List<StorageFile>();

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            DownloadOperation downloadOperation;
            String[] urlList = urls.Split(',');
            try
            {
               await Task.Run(() =>
                {
                    Parallel.ForEach<string>(urlList, async (url) =>
                    {
                        var fileName = Helper.GetFileName(url);
                        string fileExtension = url.Substring(url.LastIndexOf('.'));
                        StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                        downloadOperation = backgroundDownloader.CreateDownload(new Uri(url), file);
                        Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);
                        cancellationToken = new CancellationTokenSource();
                        await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
                    });
                });

                MessageText = "Visit here: "+ folder.Path;
                ButtonText = "CancelAll";
                ButtonClickCommand = CancelAllCmd;
            }
            catch (Exception ex)
            {

            }
        }
        private void progressChanged(DownloadOperation downloadOperation)
        {
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));

            if (progress >= 100)
            {
                downloadOperation = null;
            }
        }
        private void CancelAll(string urls)
        {
            // Save your stuff here
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
            }

            // Now switch the button   
            ButtonText = "DownloadAll";
            ButtonClickCommand = DownloadAllCmd;
        }

    }
}

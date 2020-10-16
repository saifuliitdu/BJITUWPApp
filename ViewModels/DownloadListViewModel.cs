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
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BJITUWPApp.ViewModels
{
    class DownloadListViewModel : INotifyPropertyChanged
    {
        DownloadFileService _downloadFileService;
        CancellationTokenSource cts;

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
            cts = new CancellationTokenSource();
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

        #region DisplayOperation
        private List<DownloadFileViewModel> downloadList;
        public List<DownloadFileViewModel> DownloadList
        {
            get { return downloadList; }
            set { downloadList = value; OnPropertyChanged("DownloadList"); }
        }
        private void LoadFiles()
        {
            var downloadFiles = _downloadFileService.GetFiles().Select(x=>new DownloadFileViewModel { FileName = x.FileName, Url = x.Url });
            DownloadList = new List<DownloadFileViewModel>(downloadFiles);
            Urls = string.Join(",", DownloadList.Select(x => x.Url));
        }
        #endregion

        private async void DownloadAll(string urls)
        {
            // Now switch the button   
            ButtonText = "Downloading";
            List<StorageFile> downloadedFiles = new List<StorageFile>();

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
           
            String[] urlList = urls.Split(',');
            await Task.Run(() =>
            {
                Parallel.ForEach<string>(urlList, async (url) =>
                {
                    var fileName = Helper.GetFileName(url);
                    string fileExtension = url.Substring(url.LastIndexOf('.'));
                    StorageFile file = await _downloadFileService.Download(url, fileName, fileExtension, folder);
                    downloadedFiles.Add(file);
                });
            });

            //downloadedFiles.ForEach(x =>
            //{
            //    var modelFile = DownloadList.FirstOrDefault(f => f.Url.Equals(x.Path));
            //    if (modelFile != null)
            //    {
            //        modelFile.DownloadedFile = x;
            //        modelFile.LocalFilePath = x.Path;
            //        modelFile.ButtonText = "Open";
            //        modelFile.ButtonClickCommand = modelFile.OpenCmd;
            //    }
            //});


            // Now switch the button   
            ButtonText = "CancelAll";
            ButtonClickCommand = CancelAllCmd;
        }
       
        private void CancelAll(string urls)
        {
            // Save your stuff here
            if(cts != null)
            {
                cts.Cancel();
            }

            // Now switch the button   
            ButtonText = "DownloadAll";
            ButtonClickCommand = DownloadAllCmd;
        }

    }
}

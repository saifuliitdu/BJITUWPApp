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
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BJITUWPApp.ViewModels
{
    class DownloadFileViewModel : INotifyPropertyChanged
    {
        DownloadFileService _downloadFileService;

        #region INotifyPropertyChanged_Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public DownloadFileViewModel()
        {
            _downloadFileService = new DownloadFileService();
            DownloadCmd = new DownloadCommand(Download);
            CancelCmd = new DownloadCommand(Cancel);
            OpenCmd = new DownloadCommand(Open);
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
        //public async Task DownloadAll(string urls)
        //{
        //    String[] urlList = urls.Split(',');
        //    //DownloadFileViewModel _viewModel = new DownloadFileViewModel();
        //    foreach (var url in urlList)
        //    {
        //         Download(url);
        //    }
        //}
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

            var fileName = Helper.GetFileName(url);
            string fileExtension = url.Substring(url.LastIndexOf('.'));
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            //ProgressBar.Visibility = Visibility.Visible;
            StorageFile file = await _downloadFileService.Download(url, fileName, fileExtension, folder);
            if (file != null)
            {
                downloadedFile = file;
                localFilePath = folder.Path;
                ButtonText = "Open";
                ButtonClickCommand = OpenCmd;
            }
        }
        
        private async void Open(string url)
        {
            await Windows.System.Launcher.LaunchFileAsync(downloadedFile);
            // Now switch the button   
            //ButtonText = "Download";
            //ButtonClickCommand = DownloadCmd;
        }
        private void Cancel(string url)
        {
            // Save your stuff here

            // Now switch the button   
            ButtonText = "Download";
            ButtonClickCommand = DownloadCmd;
        }

    }
}

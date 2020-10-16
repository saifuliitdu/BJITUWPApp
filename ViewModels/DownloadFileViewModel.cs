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
using Windows.UI.Xaml.Controls;

namespace BJITUWPApp.ViewModels
{
    class DownloadFileViewModel : INotifyPropertyChanged
    {
        DownloadFileService _downloadFileService;
        string status = "";

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

        public ICommand DownloadCmd
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

            string fileExtension = url.Substring(url.LastIndexOf('.'));
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            //ProgressBar.IsIndeterminate = true;
            bool isDownload = await _downloadFileService.Download(url, FileName, fileExtension, folder);
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

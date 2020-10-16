using BJITUWPApp.Commands;
using BJITUWPApp.Models;
using BJITUWPApp.Services;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BJITUWPApp.ViewModels
{
    class DownloadListViewModel : INotifyPropertyChanged
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

        public DownloadListViewModel()
        {
            _downloadFileService = new DownloadFileService();
            DownloadAllCmd = new DownloadCommand(DownloadAll);
            CancelAllCmd = new DownloadCommand(CancelAll);
            LoadFiles();
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

        private void DownloadAll(string urls)
        {
            String[] animalsArray = urls.Split(',');
            DownloadFileViewModel _viewModel = new DownloadFileViewModel();
            foreach (var url in animalsArray)
            {
                _viewModel.ButtonClickCommand.Execute(url);
            }

            // Add your stuff here

            // Now switch the button   
            ButtonText = "CancelAll";
            ButtonClickCommand = CancelAllCmd;
        }

        private void CancelAll(string urls)
        {
            // Save your stuff here

            // Now switch the button   
            ButtonText = "DownloadAll";
            ButtonClickCommand = DownloadAllCmd;
        }

    }
}

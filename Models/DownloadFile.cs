﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJITUWPApp.Models
{
    class DownloadFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged("UserName"); }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; OnPropertyChanged("Password"); }
        }
    }
}

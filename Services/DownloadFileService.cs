using BJITUWPApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BJITUWPApp.Services
{
    class DownloadFileService
    {
        List<DownloadFile> _list = null;
        DownloadFile _file;
        public List<DownloadFile> GetFiles()
        {
            _list = new List<DownloadFile>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"Files\DownloadInfo.xml");
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/FileInfo/File");
            foreach (XmlNode node in nodeList)
            {
                _file = new DownloadFile();
                _file.FileName = node.SelectSingleNode("Title").InnerText;
                _file.Url = node.SelectSingleNode("Link").InnerText;
                _list.Add(_file);
            }
            return _list;
        }

        internal async Task<bool> Download(string url, string fileName, string fileExtension, StorageFolder folder)
        {
            bool isDownload;
            try
            {
                //string fileExtension = url.Substring(url.LastIndexOf('.'));
                //FolderPicker folderPicker = new FolderPicker();
                //folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                //folderPicker.ViewMode = PickerViewMode.List;
                //folderPicker.FileTypeFilter.Add("*");
                //StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                //string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                //string path = root + @"\Assets";
                Uri adress = new Uri(url, UriKind.Absolute);
                HttpWebRequest reqst = (HttpWebRequest)WebRequest.Create(adress);
                WebResponse reponse = await reqst.GetResponseAsync();
                Stream stream = reponse.GetResponseStream();
                StorageFile file = await folder.CreateFileAsync(fileName + fileExtension, CreationCollisionOption.GenerateUniqueName);
                await Windows.Storage.FileIO.WriteBytesAsync(file, await ReadStream(stream));
                isDownload = true;
            }
            catch(Exception ex)
            {
                throw;
            }

            return isDownload;
        }

        private async Task<byte[]> ReadStream(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                   await ms.WriteAsync(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}


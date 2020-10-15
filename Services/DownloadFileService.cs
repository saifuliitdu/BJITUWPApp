using BJITUWPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    }
}

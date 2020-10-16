using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BJITUWPApp.Services
{
    public static class Helper
    {
        public static string GetFileName(string url)
        {
            var fileNameWithExt = url.Split("/").Last();
            var filename = fileNameWithExt.Substring(0, fileNameWithExt.IndexOf('.'));
            return filename;
        }
    }
}

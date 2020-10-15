using BJITUWPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BJITUWPApp.Services
{
    class UserService
    {
        public bool IsAutheticateUSer(User user)
        {
            bool isAuthUser = false;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"Files\UserInfo.xml");
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Users/User");
            string username = "", password = "";
            foreach (XmlNode node in nodeList)
            {
                username = node.SelectSingleNode("UserName").InnerText;
                password = node.SelectSingleNode("Password").InnerText;
                if (user.UserName == username && user.Password == password)
                {
                    isAuthUser = true;
                }
            }

            return isAuthUser;
        }
    }
}

using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Services.Services
{
    public class CommonService
    {
        private class Configuration
        {
            public static string ServiceSiteUrl = "https://chandraprakashtiwariv.sharepoint.com/sites/parkinglot/";
            public static string ServiceUserName = "cpt@chandraprakashtiwariv.onmicrosoft.com";
            public static string ServicePassword = "Akash@98";
        }

        public static ClientContext GetonlineContext()
        {
            var securePassword = new SecureString();
            foreach (char c in Configuration.ServicePassword)
            {
                securePassword.AppendChar(c);
            }
            var onlineCredentials = new SharePointOnlineCredentials(Configuration.ServiceUserName, securePassword);
            var context = new ClientContext(Configuration.ServiceSiteUrl);
            context.Credentials = onlineCredentials;
            return context;
        }
    }
}

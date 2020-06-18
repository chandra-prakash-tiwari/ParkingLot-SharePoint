using Microsoft.SharePoint.Client;
using ParkingLot_SharePoint.Models;
using System;

namespace ParkingLot_SharePoint.Services.Services
{
    public class UserService
    {
        public static Models.User CurrentUser { get; set; }

        private SharePointService.UserService Service { get; set; }

        public UserService(ClientContext clientContext)
        {
            this.Service = new SharePointService.UserService(clientContext);
        }

        public bool AddNewUser(Models.User user)
        {
            return this.Service.AddNewUser(user);
        }

        public bool RePassword (int id, string password)
        {
            return this.Service.ModifyUser(id, password);
        }

        public Models.User Login(Login login)
        {
            var user = this.Service.Login(login);
            if (user == null)
                return null;

            Models.User user1 = new Models.User();
            user1.Id = (int)user["ID"];
            user1.UserName = Convert.ToString(user["UserName"]);
            user1.Name = Convert.ToString(user["Title"]);
            user1.FirstTimeLogin = (bool)user["FirstTimeLogin"];
            user1.Address = Convert.ToString(user["Address"]);
            user1.Type = Convert.ToString(user["Role"]);
            var parkinglot = user["ParkingLot"] as FieldLookupValue;
            user1.ParkingLot = parkinglot.LookupId;

            return user1;
        }

        public bool FirstTimeLogin(int id, string newPassword)
        {
            return this.Service.FirstTimeLogin(id, newPassword);
        }

        public bool HasUserName(string userName)
        {
            return this.Service.HasUserName(userName);
        }
    }
}

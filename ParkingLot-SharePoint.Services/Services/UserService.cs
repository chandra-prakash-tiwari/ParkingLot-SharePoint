using Microsoft.SharePoint.Client;
using ParkingLot_SharePoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Services.Services
{
    public class UserService
    {
        private ClientContext ClientContext { get; set; }
        public static ListItem CurrentUser { get; set; }
        public UserService(ClientContext clientContext)
        {
            this.ClientContext = clientContext;
        }

        public void AddNewUser(Models.User user)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("Users");
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);
            oListItem["Title"] = user.Name;
            oListItem["Role"] = user.Type;
            oListItem["Address"] = user.Address;
            oListItem["UserName"] = user.UserName;
            oListItem["Password"] = user.Password;
            oListItem["ParkingLot"] = Convert.ToString(user.ParkingLot);
            oListItem.Update();
            this.ClientContext.ExecuteQuery();
        }

        public void ModifyUser(int id, string password)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("Users");
            ListItem oListItem = oList.GetItemById(id);
            oListItem["Password"] = password;
            oListItem.Update();
            this.ClientContext.ExecuteQuery();
        }

        public ListItem Login(Login login)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("Users");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where><And>
                                                        <Eq>
                                                            <FieldRef Name='UserName' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                        <Eq>
                                                            <FieldRef Name='Password' />
                                                            <Value Type='Text'>{1}</Value>
                                                        </Eq></And>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ID'/>
                                                    <FieldRef Name='Title'/>
                                                    <FieldRef Name='Address'/>
                                                    <FieldRef Name='Role'/>
                                                    <FieldRef Name='FirstTimeLogin'/>
                                                    <FieldRef Name='ParkingLot'/>
                                                </ViewFields>
                                                <RowLimit>1</RowLimit>
                                             </View>", login.UserName, login.Password);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();
            if (itemCollection.Count == 0)
                return null;

            else
            {
                ListItem list = itemCollection[0];
                return list;
            }
        }

        public bool FirstTimeLogin(int id, string newPassword)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("Users");
            ListItem oListItem = oList.GetItemById(id);

            oListItem["Password"] = newPassword;
            oListItem["FirstTimeLogin"] = false;

            oListItem.Update();

            this.ClientContext.ExecuteQuery();
            return true;
        }

        public bool HasUserName(string userName)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("Users");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='UserName' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ID'/>
                                                    <FieldRef Name='Title'/>
                                                    <FieldRef Name='Address'/>
                                                    <FieldRef Name='Role'/>
                                                    <FieldRef Name='FirstTimeLogin'/>
                                                    <FieldRef Name='ParkingLot'/>
                                                </ViewFields>
                                                <RowLimit>1</RowLimit>
                                             </View>", userName);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();
            if (itemCollection.Count == 0)
                return true;
            else
                return false;
        }
    }
}

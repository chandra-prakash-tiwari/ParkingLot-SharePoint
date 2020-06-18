using Microsoft.SharePoint.Client;
using ParkingLot_SharePoint.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Services.SharePointService
{
    public class ParkingService
    {
        private ClientContext ClientContext { get; set; }
        public ParkingService(ClientContext clientContext)
        {
            this.ClientContext = clientContext;
        }

        public bool ParkVehical(ParkedVehical vehical)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingInfo");
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);
            oListItem["Title"] = vehical.VehicalNumber;
            oListItem["ParkingLotSpace"] = vehical.ParkingLot;
            oListItem["ParkingLot"] = Services.UserService.CurrentUser.ParkingLot;
            oListItem["EnterTime"] = DateTime.Now;
            oListItem.Update();
            this.ClientContext.ExecuteQuery();
            return true;
        }

        public int ReleaseVehical(int id)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingInfo");
            ListItem oListItem = oList.GetItemById(id);
            oListItem["Fare"] = CalculateFare(id);
            oListItem["Manager"] = Services.UserService.CurrentUser.Id;
            oListItem["ReleaseTime"] = DateTime.Now;
            oListItem.Update();
            this.ClientContext.ExecuteQuery();
            Int32.TryParse(Convert.ToString(oListItem["Fare"]), out int fare);
            return fare;
        }

        public int CalculateFare(int id)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingInfo");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='ID' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ReleaseTime'/>
                                                    <FieldRef Name='EnterTime'/>
                                                    <FieldRef Name='ParkingLotSpace'/>
                                                </ViewFields>
                                                <RowLimit>1</RowLimit>
                                             </View>", id);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();
            if (itemCollection.Count == 0)
                return 0;

            else
            {
                ListItem list = itemCollection[0];
                TimeSpan diff = DateTime.UtcNow - (DateTime)list["EnterTime"];
                int hours = (int)diff.TotalHours+1;

                List oList2 = this.ClientContext.Web.Lists.GetByTitle("ParkingLotInfo");
                CamlQuery camlQuery2 = new CamlQuery();
                var id2 = list["ParkingLotSpace"] as FieldLookupValue;
                int id3 = id2.LookupId;
                camlQuery2.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='ID' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='Rate'/>
                                                </ViewFields>
                                                <RowLimit>1</RowLimit>
                                             </View>", id3);
                ListItemCollection itemCollection2 = oList2.GetItems(camlQuery2);
                this.ClientContext.Load(itemCollection2);
                this.ClientContext.ExecuteQuery();
                ListItem Rate = itemCollection2[0];
                Int32.TryParse(Convert.ToString(Rate["Rate"]), out int cost);
                return hours*cost;
            }
        }

        public int GetVehicalId(string vehicalNumber)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingInfo");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='Title' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ReleaseTime'/>
                                                    <FieldRef Name='EnterTime'/>
                                                    <FieldRef Name='ParkingLotSpace'/>
                                                </ViewFields>
                                             </View>", vehicalNumber, null);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();

            if (itemCollection.Count == 0)
                return 0;

            var item = itemCollection[itemCollection.Count-1];
            if (item["ReleaseTime"] == null)
            {
                Int32.TryParse(Convert.ToString(item["ID"]), out int id);
                return id;
            }
            else
            {
                return -1;
            }
        }
        public ListItem ParkingStatus(string vehicalNumber)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingInfo");

            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='Title' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ReleaseTime'/>
                                                    <FieldRef Name='EnterTime'/>
                                                    <FieldRef Name='ParkingLotSpace'/>
                                                </ViewFields>
                                             </View>", vehicalNumber, null);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();

            if (itemCollection.Count == 0)
                return null;

            var item = itemCollection[itemCollection.Count - 1];
            if (item["ReleaseTime"] != null)
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public ListItemCollection AllParkedVehical()
        {
            var parkinglot = Services.UserService.CurrentUser.ParkingLot;
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingInfo");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where><And>
                                                        <Eq>
                                                            <FieldRef Name='ParkingLot' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                        <Eq>
                                                            <FieldRef Name='IsParked' />
                                                            <Value Type='Text'>{1}</Value>
                                                        </Eq></And>                                                        
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ID'/>
                                                    <FieldRef Name='EnterTime'/>
                                                    <FieldRef Name='Title'/>
                                                </ViewFields>
                                             </View>", parkinglot, true);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();

            return itemCollection;
        }

        public int ParkingAvailable(string vehicalType)
        {
            var parkinglot = Services.UserService.CurrentUser.ParkingLot;
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingLotInfo");
            CamlQuery camlQuery = new CamlQuery
            {
                ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where><And>
                                                        <Eq>
                                                            <FieldRef Name='VehicalType' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                        <Eq>
                                                            <FieldRef Name='ParkingLotId' />
                                                            <Value Type='Number'>{1}</Value>
                                                        </Eq></And>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ParkingLotSpace'/>
                                                </ViewFields>
                                             </View>", vehicalType, parkinglot)
            };
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();

            if (itemCollection.Count == 0)
                return -1;
            else
            {
                ListItem list = itemCollection[0];
                Int32.TryParse(Convert.ToString(list["ID"]), out int id);
                return id;
            }
        }
    }
}

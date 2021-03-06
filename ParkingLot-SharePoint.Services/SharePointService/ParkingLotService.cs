﻿using Microsoft.SharePoint.Client;
using ParkingLot_SharePoint.Models;
using System;

namespace ParkingLot_SharePoint.Services.SharePointService
{
    public class ParkingLotService
    {
        private ClientContext ClientContext { get; set; }
        public ParkingLotService(ClientContext clientContext)
        {
            this.ClientContext = clientContext;
        }

        public bool AddNewParkingLot(ParkingLotsInfo parking)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingLots");
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);
            oListItem["Title"] = parking.Name;
            oListItem["Address"] = parking.Address;
            oListItem.Update();
            this.ClientContext.ExecuteQuery();
            return true;
        }

        public bool SetupParkingLot(ParkingLotInfo info)
        {
            List list = this.ClientContext.Web.Lists.GetByTitle("ParkingLotInfo");
            ListItemCreationInformation listItemCreation = new ListItemCreationInformation();
            ListItem listItem = list.AddItem(listItemCreation);
            listItem["Rate"] = info.Rate;
            listItem["Space"] = info.Space;
            listItem["VehicalType"] = info.VehicalType;
            listItem["ParkingLotId"] = info.ParkingLotId;
            listItem.Update();
            this.ClientContext.ExecuteQuery();

            return true;
        }

        public int HasParkingLot(string name)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingLots");
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
                                                    <FieldRef Name='ID'/>
                                                </ViewFields>
                                                <RowLimit>1</RowLimit>
                                             </View>", name);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();
            if (itemCollection.Count == 0)
                return 0;

            else
            {
                ListItem list = itemCollection[0];
                Int32.TryParse(Convert.ToString((int)list["ID"]), out int id);
                return id;
            }
        }

        public bool ModifyParkingLotCongiguration(ParkingLotInfo info)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingLotInfo");
            var oListItem = oList.GetItemById(info.Id);
            oListItem["Rate"] = info.Rate;
            oListItem["Space"] = info.Space;
            oListItem.Update();
            this.ClientContext.ExecuteQuery();
            return true;
        }

        public int GetParkingLotId(string vehicalType)
        {
            List oList = this.ClientContext.Web.Lists.GetByTitle("ParkingLotInfo");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(@"  
                                            <View>
                                                <Query>
                                                    <Where>
                                                        <Eq>
                                                            <FieldRef Name='ParkingLotId' />
                                                            <Value Type='Text'>{0}</Value>
                                                        </Eq>
                                                    </Where>
                                                </Query>
                                                <ViewFields>
                                                    <FieldRef Name='ID'/>
                                                </ViewFields>
                                                <RowLimit>1</RowLimit>
                                             </View>", "Bero", vehicalType);
            ListItemCollection itemCollection = oList.GetItems(camlQuery);
            this.ClientContext.Load(itemCollection);
            this.ClientContext.ExecuteQuery();
            if (itemCollection.Count == 0)
                return 0;

            else
            {
                ListItem list = itemCollection[0];
                return (int)list["ID"];
            }
        }
    }
}

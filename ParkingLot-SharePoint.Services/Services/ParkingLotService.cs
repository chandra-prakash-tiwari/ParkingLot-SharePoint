using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Services.Services
{
    public class ParkingLotService
    {
        private SharePointService.ParkingLotService Service { get; set; }

        public ParkingLotService(ClientContext clientContext)
        {
            this.Service = new SharePointService.ParkingLotService(clientContext);
        }

        public bool AddNewParkingLot(Models.ParkingLotsInfo parking)
        {
            return this.Service.AddNewParkingLot(parking);
        }

        public bool SetupParkingLot(Models.ParkingLotInfo info)
        {
            return this.Service.SetupParkingLot(info);
        }

        public int HasParkingLot(string name)
        {
            return this.Service.HasParkingLot(name);
        }

        public bool ModifyParkingLotCongiguration(Models.ParkingLotInfo info)
        {
            return this.Service.ModifyParkingLotCongiguration(info);
        }

        public int GetParkingLotId(string vehicleType)
        {
            return this.Service.GetParkingLotId(vehicleType);
        }
    }
}

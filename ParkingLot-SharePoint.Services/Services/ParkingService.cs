using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Services.Services
{
    public class ParkingService
    {
        private SharePointService.ParkingService Service { get; set; }

        public ParkingService(ClientContext clientContext)
        {
            this.Service = new SharePointService.ParkingService(clientContext);
        }

        public bool ParkVehical(Models.ParkedVehical vehical)
        {
            return this.Service.ParkVehical(vehical);
        }

        public int ReleaseVehical(int id)
        {
            return this.Service.ReleaseVehical(id);
        }

        public int GetVehicalId(string vehicleNumber)
        {
            return this.Service.GetVehicalId(vehicleNumber);
        }

        public Models.ParkedVehical ParkingStatus(string vehicleNumber)
        {
            var parking = this.Service.ParkingStatus(vehicleNumber);

            if (parking == null)
                return null;

            Models.ParkedVehical vehical = new Models.ParkedVehical
            {
                ReleaseTime = (DateTime)parking["ReleaseTime"],
                EnteyTime = (DateTime)parking["EnterTime"],
                ParkingLot = (int)parking["ParkingLotSpace"]
            };

            return vehical;
        }

        public List<Models.ParkedVehical> AllParkedVehical()
        {
            var vehicles = this.Service.AllParkedVehical();
            if (vehicles == null)
                return null;

            List<Models.ParkedVehical> parkedVehicals = new List<Models.ParkedVehical>();
            foreach(var vehicle in vehicles)
            {
                Models.ParkedVehical parkedVehical = new Models.ParkedVehical
                {
                    VehicalNumber = Convert.ToString(vehicle["Title"]),
                    EnteyTime = (DateTime)vehicle["EnterTime"],
                };

                parkedVehicals.Add(parkedVehical);
            }

            return parkedVehicals;
        }

        public int ParkingAvailable(string vehicleType)
        {
            return this.Service.ParkingAvailable(vehicleType);
        }
    }
}

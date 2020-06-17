using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Models
{
    public class ParkedVehical
    {
        public string Id { get; set; }

        public string VehicalNumber { get; set; }

        public DateTime EnteyTime { get; set; }

        public DateTime ReleaseTime { get; set; }

        public int ParkingLot { get; set; }

        public int FareCollectedManagerId { get; set; }

        public int Fare { get; set; }
    }
}

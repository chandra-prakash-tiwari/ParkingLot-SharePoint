using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Models
{
    public class ParkingLotInfo
    {
        public int Id { get; set; }

        public string VehicalType { get; set; }

        public int Rate { get; set; }

        public int ParkingLotId { get; set; }

        public int Space { get; set; }
    }
}

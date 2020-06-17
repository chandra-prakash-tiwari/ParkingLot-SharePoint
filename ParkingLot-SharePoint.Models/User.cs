using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot_SharePoint.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public bool FirstTimeLogin { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        public int ParkingLot { get; set; }
    }
}

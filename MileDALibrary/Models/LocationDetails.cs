using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class LocationDetails
    {
        public int? User_id { get; set; }

        public int? Location_id { get; set; }

        public string? Location_type { get; set; }

        public string? Location_address { get; set; }

        public string? Location_street { get; set; }

        public string? Pincode { get; set; }

        public string? Location_landmark { get; set; }

        public string? Status { get; set; }

    }
}

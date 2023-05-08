using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class DriversCurrLocation
    {
        public int? Driver_Id { get; set; }
        public string? Location_Name { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int Created_UserId { get; set; }
        //public decimal Query_Name { get; set; }
        //public decimal Response_Status { get; set; }
    }
}

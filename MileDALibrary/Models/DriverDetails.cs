using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class DriverDetails
    {
        public string? Name { get; set; }

        public string? Gender { get; set; }

        public string? Phone_num { get; set; }

        public string? Email_id { get; set; }

        public string? Address { get; set; }
        
        public string? Date_of_birth { get; set; }
        
        public int? Vehicle_type_id { get; set; }
        
        public string? Vehicle_type_name { get; set; }
        
        public string? License_no { get; set; }
        
        public string? License_plate_no { get; set; }
        
        public string? Insurance_no { get; set; }
        
        public string? Aadhar_no { get; set; }

        public string? Usr_state { get; set; }

        public string? Usr_district { get; set; }
    }
}

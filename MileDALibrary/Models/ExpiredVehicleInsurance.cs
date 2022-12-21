using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class ExpiredVehicleInsurance
    {
        public int User_id { get; set; }
        public string? First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Email_id { get; set; }
        public string? Phone_num { get; set; }
        public string? Notification_token { get; set; }
        public string? License_plate_no { get; set; }
        public string? Expiry_date { get; set; }
        public string? Msg { get; set; }
        public string? flag { get; set; }
    }

    public class ExpiredVehicleDetails
    {
        public int? Userid { get; set; }
        public string? Phonenum { get; set; }
        public string? Emailid { get; set; }
        public string? Userpassword { get; set; }
        public string? VehicleLicenseNo { get; set; }
        public string? DriverName { get; set; }
        public string? QueryName { get; set; }
        public string? ResponseStatus { get; set; }

    }
}

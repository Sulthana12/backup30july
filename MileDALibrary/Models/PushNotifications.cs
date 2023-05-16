using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class PushNotifications
    {
        public int? User_id { get; set; }

        public int? User_track_id { get; set; }

        public string? User_Name { get; set; }

        public string? Fare_status { get; set; }
        public string? User_Phone_Num { get; set; }

        public string? Driver_Name { get; set; }
        public string? Driver_Phone_Num { get; set; }
        public string? Driver_Email { get; set; }
        public string? User_Email { get; set; }
        public string? title { get; set; }

        public string? body { get; set; }
        public string? notification_token { get; set; }
        public int? Routed_driver_id { get; set; }

      
    }
}

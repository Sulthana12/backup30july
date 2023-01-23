using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class UpdateProfile
    {
        public string? First_name { get; set; }

        public string? Last_name { get; set; }

        public string? Email_id { get; set; }

        public string? Phone_num { get; set; }

        public string? User_Password { get; set; }

        public string? User_type_flg { get; set; }

        public string? En_flg { get; set; }

        public string? Notification_token { get; set; }

        public int? User_id { get; set; }
        public string? Image_data { get; set; }

        public string? Usr_img_file_name { get; set; }
        
        public string? Usr_img_file_location { get; set; }
        
    }
}

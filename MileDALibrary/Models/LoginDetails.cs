using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class LoginDetails
    {
        public int? User_id { get; set; }

        public string? Phone_num { get; set; }

        public string? Email_id { get; set; }

        public string? User_type_flg { get; set; }

        public string? Name { get; set; }

        public string? Notification_token { get; set; }
        public string? Referral_code { get; set; }
    }
}

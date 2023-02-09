using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class UserByPhoneOrEmail
    {
        public int? user_id { get; set; }
        public string? name { get; set; }
        public string? phone_num { get; set; }
        public string? email_id { get; set; }
        public string? user_type_flg { get; set; }
    }
}

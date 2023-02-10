using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class PwdUpdate
    {
        public int? User_id { get; set; }

        public string? Phone_num { get; set; }

        public string? User_password { get; set; }

        public string? User_type_flg { get; set; }

    }
}

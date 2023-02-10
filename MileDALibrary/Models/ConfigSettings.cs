using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class ConfigSettings
    {
        public int? Settings_id { get; set; }

        public string? Settings_name { get; set; }

        public string? Settings_value { get; set; }

        public string? Setting_desc { get; set; }
        public string? Type { get; set; }
        public string? Days { get; set; }
        public string? file_name { get; set; }
        public string? file_location { get; set; }

        public string? En_flg { get; set; }
    }
}

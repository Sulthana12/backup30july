using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class DriverRegPaymentStatus
    {
        public int? User_id { get; set; }
        public string? Phone_num { get; set; }
        public decimal? Amount { get; set; }
        public string? Status_flg { get; set; }
        public string? Payment_id { get; set; }
        public string? Screen_type { get; set; }

    }
}

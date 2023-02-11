using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
	public class ReferralDetails
	{
		public int? user_id { get; set; }

		public string? referral_code { get; set; }

        public string? Bank_mobile_num { get; set; }

		public string? User_type_flg { get; set; }
    }
}

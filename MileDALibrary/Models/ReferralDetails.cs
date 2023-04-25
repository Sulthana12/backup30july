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
        public int? driver_id { get; set; }

        public string? referral_code { get; set; }

        public string? Bank_mobile_num { get; set; }

		public string? User_type_flg { get; set; }
		public string? User_Phone_Num { get; set; }
		public decimal? Email_Id { get; set; }
        public decimal? Driver_Latitude { get; set; }
        public decimal? Driver_Longitude { get; set; }
        public decimal? Fare { get; set; }
        public decimal? Fare_Requested_In_Kms { get; set; }
		public	string? Driver_Location_Name { get; set; }
        public decimal? User_Latitude { get; set; }
        public decimal? User_Longitude { get; set; }
        public string? User_Location_Name { get; set; }
        public string? User_Name { get; set; }

        public string? Driver_Name { get; set; }
        public string? Driver_Photo { get; set; }
        public decimal? Driver_Rating { get; set; }
        public string? Vehicle_No { get; set; }
        public int? OTP { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class DriverPaymentDetails
    {
        public int? User_Id { get; set; }

        public string? Bank_Name { get; set; }

        public string? Branch_Name { get; set; }

        public string? Ifsc_Code { get; set; }

        public string? Bank_Img_File_Name { get; set; }

        public string? Bank_Img_File_Location { get; set; }

        public string? Bank_Img_File_Data { get; set; }

        public string? Account_Number { get; set; }

        public string? Bank_mobile_num { get; set; }
    }
}

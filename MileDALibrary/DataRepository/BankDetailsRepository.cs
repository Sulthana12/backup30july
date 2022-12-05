using MileDALibrary.Interfaces;
using MileDALibrary.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.DataRepository
{
    public class BankDetailsRepository : IBankDetailsRepository
    {
        public async Task<BankDetails> GetBankDetails(string ifscCode)
        {
            using (var client = new HttpClient())
            {
                var url = new Uri($"https://demo.infomattic.com/api/ifsc-bank/api_request.php?ifsc="+ifscCode);
                HttpResponseMessage response = client.GetAsync(url.ToString()).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                List<BankDetails> bankDetails = new List<BankDetails>();
                JObject json = JObject.Parse(result);
                string data = json["data"].ToString();
                if (data != "Null")
                {
                    bankDetails.Add(new BankDetails
                    {
                        MICR = (string)json["data"]["MICR"] == null ? "" : (string)json["data"]["MICR"],
                        BRANCH = (string)json["data"]["BRANCH"] == null ? "" : (string)json["data"]["BRANCH"],
                        ADDRESS = (string)json["data"]["ADDRESS"] == null ? "" : (string)json["data"]["ADDRESS"],
                        CONTACT = (string)json["data"]["CONTACT"] == null ? "" : (string)json["data"]["CONTACT"],
                        UPI = (string)json["data"]["UPI"] == null ? "" : (string)json["data"]["UPI"],
                        STATE = (string)json["data"]["STATE"] == null ? "" : (string)json["data"]["STATE"],
                        RTGS = (string)json["data"]["RTGS"] == null ? "" : (string)json["data"]["RTGS"],
                        CITY = (string)json["data"]["CITY"] == null ? "" : (string)json["data"]["CITY"],
                        CENTRE = (string)json["data"]["CENTRE"] == null ? "" : (string)json["data"]["CENTRE"],
                        DISTRICT = (string)json["data"]["DISTRICT"] == null ? "" : (string)json["data"]["DISTRICT"],
                        NEFT = (string)json["data"]["NEFT"] == null ? "" : (string)json["data"]["NEFT"],
                        IMPS = (string)json["data"]["IMPS"] == null ? "" : (string)json["data"]["IMPS"],
                        SWIFT = (string)json["data"]["SWIFT"] == null ? "" : (string)json["data"]["SWIFT"],
                        ISO3166 = (string)json["data"]["ISO3166"] == null ? "" : (string)json["data"]["ISO3166"],
                        BANK = (string)json["data"]["BANK"] == null ? "" : (string)json["data"]["BANK"],
                        BANKCODE = (string)json["data"]["BANKCODE"] == null ? "" : (string)json["data"]["BANKCODE"],
                        IFSC = (string)json["data"]["IFSC"] == null ? "" : (string)json["data"]["IFSC"],
                    });
                }
                return (bankDetails[0]);
            }
        }

    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MileDALibrary.Interfaces;
using MileDALibrary.Helper;
using MileDALibrary.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Azure.Storage.Blobs;
using System.Drawing;
using Azure.Communication;
using Azure.Communication.Sms;
using MimeKit;
using System.Net.Mail;
using MailKit.Security;
//using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using System.Reflection;
using MailKit.Search;
using Azure.Core;

namespace MileDALibrary.DataRepository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly IOptions<BlobConfig> blobconfig;
        private readonly IOptions<DBSettings> options;
        private readonly IOptions<SMTPGateway> _SMTPGateway;
        string istStrDate = "select CAST(DATEADD(HOUR, 5, DATEADD(MINUTE, 30, GETUTCDATE())) as DATE)";
        private string istDate = "";

        public UserRepository(IConfiguration config, IOptions<BlobConfig> blobconfig, IOptions<DBSettings> options) : base(config)
        {
            this.blobconfig = blobconfig;
            this.options = options;
            SQL_Helper.SetConnectionString(this.options.Value.ConnectionString);
            this.istDate = GetDateFromServer();
        }

        public string GetDateFromServer()
        {
            DataTable dt = new DataTable();
            dt = SQL_Helper.ExecuteSelect<SqlConnection>(istStrDate, null, SQL_Helper.ExecutionType.Query);
            return dt.Rows[0][0].ToString();
        }

        public List<LoginDetails> GetUserInformation(string PhoneNumber, string Password)
        {
            List<LoginDetails> UserResponse = new List<LoginDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "Login", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@phone_num", Value = PhoneNumber, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_password", Value = Password, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new LoginDetails()
                                {
                                    User_id = Convert.ToInt32(dr["user_id"]),
                                    Phone_num = dr["phone_num"].ToString(),
                                    Email_id = dr["email_id"].ToString(),
                                    User_type_flg = dr["user_type_flg"].ToString(),
                                    Name = dr["name"].ToString(),
                                    Notification_token = dr["notification_token"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<CountryDetails> GetCountryDetails()
        {
            List<CountryDetails> countryDetails = new List<CountryDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparams = new List<DbParameter>();
            dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrcountry", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparams, SQL_Helper.ExecutionType.Procedure);
            if (dt != null && dt.Rows.Count > 0)
            {
                countryDetails = (from DataRow dr in dt.Rows
                                  select new CountryDetails()
                                  {
                                      Country_name = dr["country_name"].ToString(),
                                      Countryid = Convert.ToInt32(dr["country_id"])
                                  }).ToList();
            }

            return countryDetails;
        }

        public List<StateDetails> GetStateDetails()
        {
            List<StateDetails> stateDetails = new List<StateDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparams = new List<DbParameter>();
            dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrstate", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparams, SQL_Helper.ExecutionType.Procedure);
            if (dt != null && dt.Rows.Count > 0)
            {
                stateDetails = (from DataRow dr in dt.Rows
                                select new StateDetails()
                                {
                                    State_name = dr["state_name"].ToString(),
                                    State_id = Convert.ToInt32(dr["state_id"]),
                                    Country_id = Convert.ToInt32(dr["country_id"]),
                                }).ToList();
            }

            return stateDetails;
        }

        public List<DistrictDetails> GetDistrictDetails(int stateId, int countryId)
        {
            List<DistrictDetails> UserResponse = new List<DistrictDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrdistrict", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@state_id", Value = stateId, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@country_id", Value = countryId, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new DistrictDetails()
                                {
                                    District_id = Convert.ToInt32(dr["district_id"]),
                                    District_name = dr["district_name"].ToString(),
                                    State_id = Convert.ToInt32(dr["state_id"]),
                                    State_name = dr["state_name"].ToString(),
                                    Country_id = Convert.ToInt32(dr["country_id"]),
                                    Country_name = dr["country_name"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<VehicleDetails> GetVehicleDetails()
        {
            List<VehicleDetails> VehicleDetails = new List<VehicleDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparams = new List<DbParameter>();
            dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrvehicles", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparams, SQL_Helper.ExecutionType.Procedure);
            if (dt != null && dt.Rows.Count > 0)
            {
                VehicleDetails = (from DataRow dr in dt.Rows
                                  select new VehicleDetails()
                                  {
                                      Vehicle_type_id = Convert.ToInt32(dr["vehicle_type_id"]),
                                      Vehicle_type_name = dr["vehicle_type_name"].ToString(),
                                      En_flg = dr["en_flg"].ToString(),
                                  }).ToList();
            }

            return VehicleDetails;
        }

        public List<GenderDetails> GetGenderDetails(string settingsName)
        {
            List<GenderDetails> UserResponse = new List<GenderDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrgender", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@settings_name", Value = settingsName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new GenderDetails()
                                {
                                    Settings_id = Convert.ToInt32(dr["settings_id"]),
                                    Settings_name = dr["settings_name"].ToString(),
                                    Settings_value = dr["settings_value"].ToString(),
                                    Setting_desc = dr["setting_desc"].ToString(),
                                    En_flg = dr["en_flg"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public async Task<List<ResponseStatus>> UpdateProfileDetails(UpdateProfile updateProfile)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();

            BlobServiceClient blobServiceClient = new BlobServiceClient(this.blobconfig.Value.BlobConnection);

            try
            {
                if (!string.IsNullOrEmpty(updateProfile.Image_data))
                {
                    //string imagedata = UserRepository.ScaleImage(updateProfile.Image_data, 140, 140);
                    //updateProfile.Image_data = string.Empty;
                    //updateProfile.Image_data = imagedata;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = updateProfile.First_name + "-" + updateProfile.User_id + "-" + "image.jpg";
                    blobEntity.ByteArray = updateProfile.Image_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response1 = await blobClient.UploadAsync(stream, true);

                    updateProfile.Usr_img_file_location = this.blobconfig.Value.UserProfilePhoto;
                    updateProfile.Usr_img_file_name = blobEntity.FolderName;

                }

                if (updateProfile != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "updateprofile", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@first_name", Value = updateProfile.First_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@last_name", Value = updateProfile.Last_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@email_id", Value = updateProfile.Email_id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@phone_num", Value = updateProfile.Phone_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_password", Value = updateProfile.User_Password, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_type_flg", Value = updateProfile.User_type_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@en_flg", Value = updateProfile.En_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@usr_img_file_name", Value = String.IsNullOrEmpty(updateProfile.Usr_img_file_name) ? DBNull.Value : (object)updateProfile.Usr_img_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@usr_img_file_location", Value = String.IsNullOrEmpty(updateProfile.Usr_img_file_location) ? DBNull.Value : (object)updateProfile.Usr_img_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@notification_token", Value = updateProfile.Notification_token, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = updateProfile.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@screen_type", Value = String.IsNullOrEmpty(updateProfile.Screen_type) ? DBNull.Value : (object)updateProfile.Screen_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@template_id", Value = String.IsNullOrEmpty(updateProfile.Referral_Code) ? DBNull.Value : (object)updateProfile.Referral_Code, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });
                    dbparams.Add(new SqlParameter { ParameterName = "@error_user_id", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    string spOut1 = DBNull.Value.Equals(result["@error_user_id"]) ? "" : result["@error_user_id"];

                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;
                        respobj.OutUserId = spOut1;

                        response.Add(respobj);


                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<ResponseStatus> DriverRegPaymentStatusDetails(DriverRegPaymentStatus DriverRegPaymentStatus)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
                if (DriverRegPaymentStatus != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "DriverRegPaymentStatus", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@User_id", Value = DriverRegPaymentStatus.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@Bank_mobile_num", Value = DriverRegPaymentStatus.Phone_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@Amount", Value = DriverRegPaymentStatus.Amount, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@status_flg", Value = DriverRegPaymentStatus.Status_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@payment_id", Value = DriverRegPaymentStatus.Payment_id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@screen_type", Value = DriverRegPaymentStatus.Screen_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_taxi_driver_payment_Post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;

                        response.Add(respobj);


                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<DriverDetails> GetDriverDetails(string phoneNumber)
        {
            List<DriverDetails> UserResponse = new List<DriverDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetDriverDetails", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@phone_num", Value = phoneNumber, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new DriverDetails()
                                {
                                    Name = dr["name"].ToString(),
                                    Gender = dr["gender"].ToString(),
                                    Phone_num = dr["phone_num"].ToString(),
                                    Email_id = dr["email_id"].ToString(),
                                    Address = dr["usr_addr"].ToString(),
                                    Date_of_birth = dr["date_of_birth"].ToString(),
                                    Vehicle_type_id = dr["vehicle_type_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["vehicle_type_id"]),
                                    Vehicle_type_name = dr["vehicle_type_name"].ToString(),
                                    License_no = dr["license_no"].ToString(),
                                    License_plate_no = dr["license_plate_no"].ToString(),
                                    Insurance_no = dr["insurance_no"].ToString(),
                                    Aadhar_no = dr["aadhar_no"].ToString(),
                                    Usr_state = dr["usr_state"].ToString(),
                                    Usr_district = dr["usr_district"].ToString(),
                                    Country = dr["usr_country"].ToString(),
                                    Pincode = dr["pincode"].ToString(),
                                    Usr_img_file_location = dr["usr_img_file_location"].ToString(),
                                    Usr_img_file_name = dr["usr_img_file_name"].ToString(),
                                    User_address = dr["usr_addr"].ToString(),
                                    En_flag = dr["en_flg"].ToString(),
                                    Aadhar_file_name = dr["aadhar_file_name"].ToString(),
                                    Aadhar_file_location = dr["aadhar_file_location"].ToString(),
                                    Drv_pan_no = dr["drv_pan_no"].ToString(),
                                    Panno_file_name = dr["panno_file_name"].ToString(),
                                    Panno_file_location = dr["panno_file_location"].ToString(),
                                    Licno_file_name = dr["licno_file_name"].ToString(),
                                    Licno_file_location = dr["licno_file_location"].ToString(),
                                    insno_file_name = dr["insno_file_name"].ToString(),
                                    Insno_file_location = dr["insno_file_location"].ToString(),
                                    Plateno_file_name = dr["plateno_file_name"].ToString(),
                                    Plateno_file_location = dr["plateno_file_location"].ToString(),
                                    Doc_file_name = dr["doc_file_name"].ToString(),
                                    Doc_file_location = dr["doc_file_location"].ToString(),
                                    User_id = dr["user_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["user_id"]),
                                    First_Name = dr["first_name"].ToString(),
                                    Last_Name = dr["last_name"].ToString(),
                                    District_id = dr["district_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["district_id"]),
                                    Id_proof_name = dr["id_proof_name"].ToString(),
                                    Comments = dr["Comments"].ToString(),
                                    Bank_Name = dr["Bank_name"].ToString(),
                                    Branch_Name = dr["branch_name"].ToString(),
                                    Ifsc_Code = dr["ifsc_code"].ToString(),
                                    Bank_Img_File_Name = dr["bank_img_file_name"].ToString(),
                                    Bank_Img_File_Location = dr["bank_img_file_location"].ToString(),
                                    Account_Number = dr["Account_number"].ToString(),
                                    Bank_mobile_num = dr["bank_mobile_num"].ToString(),
                                    Driving_License_Expiry_Date = dr["Driving_license_Expiry_date"].ToString(),
                                    Vehicle_Insurance_Expiry_Date = dr["Vehicle_insurance_Expiry_date"].ToString(),
                                    Regstr_pymt_flg = dr["Regstr_pymt_flg"].ToString(),
                                    referral_code = dr["referral_code"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public async Task<List<ResponseStatus>> SaveUserDetails(UserDetails userDetails)
        {

            int insertRowsCount = 0;
            List<ResponseStatus> output = new List<ResponseStatus>();
            BlobServiceClient blobServiceClient = new BlobServiceClient(this.blobconfig.Value.BlobConnection);

            try
            {
                if (!string.IsNullOrEmpty(userDetails.Image_data))
                {


                    //string imagedata = UserRepository.ScaleImage(userDetails.Image_data, 140, 140);

                    //userDetails.Image_data = string.Empty;
                    //userDetails.Image_data = imagedata;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "image.jpg";
                    blobEntity.ByteArray = userDetails.Image_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Usr_img_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Usr_img_file_name = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.Doc_data))
                {


                    string docData = userDetails.Doc_data;

                    userDetails.Doc_data = string.Empty;
                    userDetails.Doc_data = docData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "doc.pdf";
                    blobEntity.ByteArray = userDetails.Doc_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Doc_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Doc_file_name = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.Aadhar_data))
                {


                    //string aadharData = UserRepository.ScaleImage(userDetails.Aadhar_data, 140, 140);

                    //userDetails.Aadhar_data = string.Empty;
                    //userDetails.Aadhar_data = aadharData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "aadhar.jpg";
                    blobEntity.ByteArray = userDetails.Aadhar_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Aadhar_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Aadhar_file_name = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.Pan_data))
                {


                    //string panData = UserRepository.ScaleImage(userDetails.Pan_data, 140, 140);

                    //userDetails.Pan_data = string.Empty;
                    //userDetails.Pan_data = panData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "pan.jpg";
                    blobEntity.ByteArray = userDetails.Pan_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Panno_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Panno_file_name = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.License_data))
                {


                    //string licenseData = UserRepository.ScaleImage(userDetails.License_data, 140, 140);

                    //userDetails.License_data = string.Empty;
                    //userDetails.License_data = licenseData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "license.jpg";
                    blobEntity.ByteArray = userDetails.License_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Licno_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Licno_file_name = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.Insurance_data))
                {


                    //string insuranceData = UserRepository.ScaleImage(userDetails.Insurance_data, 140, 140);

                    //userDetails.Insurance_data = string.Empty;
                    //userDetails.Insurance_data = insuranceData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "insurance.jpg";
                    blobEntity.ByteArray = userDetails.Insurance_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Insno_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.insno_file_name = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.PlateNo_data))
                {


                    //string plateNoData = UserRepository.ScaleImage(userDetails.PlateNo_data, 140, 140);

                    //userDetails.PlateNo_data = string.Empty;
                    //userDetails.PlateNo_data = plateNoData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "plateno.jpg";
                    blobEntity.ByteArray = userDetails.PlateNo_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Plateno_file_location = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Plateno_file_name = blobEntity.FolderName;

                }


                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

                List<DbParameter> dbparamsUserInfo = new List<DbParameter>();

                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "adddrvinfo", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@new_user_id", Value = userDetails.New_userid, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@first_name", Value = String.IsNullOrEmpty(userDetails.First_name) ? DBNull.Value : (object)userDetails.First_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@last_name", Value = String.IsNullOrEmpty(userDetails.Last_name) ? DBNull.Value : (object)userDetails.Last_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@gender", Value = String.IsNullOrEmpty(userDetails.Gender) ? DBNull.Value : (object)userDetails.Gender, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@phone_num", Value = String.IsNullOrEmpty(userDetails.Phone_number) ? DBNull.Value : (object)userDetails.Phone_number, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@usr_addr", Value = String.IsNullOrEmpty(userDetails.User_address) ? DBNull.Value : (object)userDetails.User_address, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@email_id", Value = String.IsNullOrEmpty(userDetails.Email_id) ? DBNull.Value : (object)userDetails.Email_id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_password", Value = String.IsNullOrEmpty(userDetails.User_password) ? DBNull.Value : (object)userDetails.User_password, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@date_of_birth", Value = String.IsNullOrEmpty(userDetails.Date_of_birth) ? DBNull.Value : (object)userDetails.Date_of_birth, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_type_flg", Value = String.IsNullOrEmpty(userDetails.User_type_flg) ? DBNull.Value : (object)userDetails.User_type_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@usr_img_file_name", Value = String.IsNullOrEmpty(userDetails.Usr_img_file_name) ? DBNull.Value : (object)userDetails.Usr_img_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@usr_img_file_location", Value = String.IsNullOrEmpty(userDetails.Usr_img_file_location) ? DBNull.Value : (object)userDetails.Usr_img_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@drv_license_no", Value = String.IsNullOrEmpty(userDetails.Drv_license_no) ? DBNull.Value : (object)userDetails.Drv_license_no, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@aadhar_no", Value = String.IsNullOrEmpty(userDetails.Aadhar_no) ? DBNull.Value : (object)userDetails.Aadhar_no, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@country", Value = String.IsNullOrEmpty(userDetails.Country) ? DBNull.Value : (object)userDetails.Country, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@state", Value = String.IsNullOrEmpty(userDetails.State) ? DBNull.Value : (object)userDetails.State, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@district", Value = String.IsNullOrEmpty(userDetails.District) ? DBNull.Value : (object)userDetails.District, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@pincode", Value = String.IsNullOrEmpty(userDetails.Pincode) ? DBNull.Value : (object)userDetails.Pincode, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = userDetails.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@en_flg", Value = String.IsNullOrEmpty(userDetails.En_flag) ? DBNull.Value : (object)userDetails.En_flag, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@vehicle_type_id", Value = userDetails.Vehicle_type_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@license_plate_no", Value = String.IsNullOrEmpty(userDetails.License_plate_no) ? DBNull.Value : (object)userDetails.License_plate_no, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@drv_insurance_no", Value = String.IsNullOrEmpty(userDetails.Drv_insurance_no) ? DBNull.Value : (object)userDetails.Drv_insurance_no, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@drv_aadhar_no", Value = String.IsNullOrEmpty(userDetails.Drv_aadhar_no) ? DBNull.Value : (object)userDetails.Drv_aadhar_no, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@doc_file_name", Value = String.IsNullOrEmpty(userDetails.Doc_file_name) ? DBNull.Value : (object)userDetails.Doc_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@doc_file_location", Value = String.IsNullOrEmpty(userDetails.Doc_file_location) ? DBNull.Value : (object)userDetails.Doc_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@aadhar_file_name", Value = String.IsNullOrEmpty(userDetails.Aadhar_file_name) ? DBNull.Value : (object)userDetails.Aadhar_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@aadhar_file_location", Value = String.IsNullOrEmpty(userDetails.Aadhar_file_location) ? DBNull.Value : (object)userDetails.Aadhar_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@drv_pan_no", Value = String.IsNullOrEmpty(userDetails.Drv_pan_no) ? DBNull.Value : (object)userDetails.Drv_pan_no, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@panno_file_name", Value = String.IsNullOrEmpty(userDetails.Panno_file_name) ? DBNull.Value : (object)userDetails.Panno_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@panno_file_location", Value = String.IsNullOrEmpty(userDetails.Panno_file_location) ? DBNull.Value : (object)userDetails.Panno_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@licno_file_name", Value = String.IsNullOrEmpty(userDetails.Licno_file_name) ? DBNull.Value : (object)userDetails.Licno_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@licno_file_location", Value = String.IsNullOrEmpty(userDetails.Licno_file_location) ? DBNull.Value : (object)userDetails.Licno_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@insno_file_name", Value = String.IsNullOrEmpty(userDetails.insno_file_name) ? DBNull.Value : (object)userDetails.insno_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@insno_file_location", Value = String.IsNullOrEmpty(userDetails.Insno_file_location) ? DBNull.Value : (object)userDetails.Insno_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@plateno_file_name", Value = String.IsNullOrEmpty(userDetails.Plateno_file_name) ? DBNull.Value : (object)userDetails.Plateno_file_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@plateno_file_location", Value = String.IsNullOrEmpty(userDetails.Plateno_file_location) ? DBNull.Value : (object)userDetails.Plateno_file_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@district_id", Value = userDetails.District_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@id_proof_name", Value = String.IsNullOrEmpty(userDetails.Id_Proof_Name) ? DBNull.Value : (object)userDetails.Id_Proof_Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@comments", Value = String.IsNullOrEmpty(userDetails.Comments) ? DBNull.Value : (object)userDetails.Comments, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@Driving_license_Expiry_date", Value = String.IsNullOrEmpty(userDetails.Driving_License_Expiry_Date) ? DBNull.Value : (object)userDetails.Driving_License_Expiry_Date, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@Vehicle_insurance_Expiry_date", Value = String.IsNullOrEmpty(userDetails.Vehicle_Insurance_Expiry_Date) ? DBNull.Value : (object)userDetails.Vehicle_Insurance_Expiry_Date, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

                insertRowsCount = insertRowsCount + result["RowsAffected"];

                string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                if (!string.IsNullOrEmpty(spOut))
                {
                    ResponseStatus respobj = new ResponseStatus();
                    respobj.Error_desc = spOut;

                    output.Add(respobj);
                }

                return output;
            }
            catch (Exception)
            {
                return output;
            }
        }

        public async Task<List<ResponseStatus>> DriverPaymentDetails(DriverPaymentDetails driverPaymentDetails)
        {

            int insertRowsCount = 0;
            List<ResponseStatus> output = new List<ResponseStatus>();
            BlobServiceClient blobServiceClient = new BlobServiceClient(this.blobconfig.Value.BlobConnection);

            try
            {


                if (!string.IsNullOrEmpty(driverPaymentDetails.Bank_Img_File_Data))
                {


                    //string bankdata = UserRepository.ScaleImage(driverPaymentDetails.Bank_Img_File_Data, 140, 140);

                    //driverPaymentDetails.Bank_Img_File_Data = string.Empty;
                    //driverPaymentDetails.Bank_Img_File_Data = bankdata;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = driverPaymentDetails.User_Id + "-" + "BankDetails.jpg";
                    blobEntity.ByteArray = driverPaymentDetails.Bank_Img_File_Data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    driverPaymentDetails.Bank_Img_File_Location = this.blobconfig.Value.UserProfilePhoto;
                    driverPaymentDetails.Bank_Img_File_Name = blobEntity.FolderName;

                }


                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

                List<DbParameter> dbparamsUserInfo = new List<DbParameter>();

                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "drvpaymentdtls", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = driverPaymentDetails.User_Id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@bank_name", Value = String.IsNullOrEmpty(driverPaymentDetails.Bank_Name) ? DBNull.Value : (object)driverPaymentDetails.Bank_Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@branch_name", Value = String.IsNullOrEmpty(driverPaymentDetails.Branch_Name) ? DBNull.Value : (object)driverPaymentDetails.Branch_Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@ifsc_code", Value = String.IsNullOrEmpty(driverPaymentDetails.Ifsc_Code) ? DBNull.Value : (object)driverPaymentDetails.Ifsc_Code, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@Account_number", Value = String.IsNullOrEmpty(driverPaymentDetails.Account_Number) ? DBNull.Value : (object)driverPaymentDetails.Account_Number, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@bank_mobile_num", Value = String.IsNullOrEmpty(driverPaymentDetails.Bank_mobile_num) ? DBNull.Value : (object)driverPaymentDetails.Bank_mobile_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

                insertRowsCount = insertRowsCount + result["RowsAffected"];

                string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                if (!string.IsNullOrEmpty(spOut))
                {
                    ResponseStatus respobj = new ResponseStatus();
                    respobj.Error_desc = spOut;

                    output.Add(respobj);
                }

                return output;
            }
            catch (Exception)
            {
                return output;
            }
        }

        public static string ScaleImage(string ImageData, int maxWidth, int maxHeight)
        {
            System.Drawing.Image image;

            byte[] bytes = System.Convert.FromBase64String(ImageData);
            string base64;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = System.Drawing.Image.FromStream(ms);
            }

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            using (MemoryStream m = new MemoryStream())
            {
                newImage.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = m.ToArray();
                base64 = Convert.ToBase64String(imageBytes);

            }

            return base64;
        }

        public int SendEmail(string emailId, int otp)
        {
            try
            {
                using (MailMessage mm = new MailMessage("afartechnologiesmdu@gmail.com", emailId))
                {
                    mm.Subject = "AFAR-Cabing Service | OTP to Verify Email";
                    mm.Body = "Hello Partner," + System.Environment.NewLine + System.Environment.NewLine + "AFAR CABS requires further verification" + System.Environment.NewLine + System.Environment.NewLine + "To complete the sign in, enter the verification code:" + System.Environment.NewLine + System.Environment.NewLine + otp.ToString() + System.Environment.NewLine + System.Environment.NewLine + "The verification code will be valid for 1 minute.Please do not share this code with anyone.";
                    mm.IsBodyHtml = false;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.UseDefaultCredentials = false;
                        NetworkCredential NetworkCred = new NetworkCredential("afartechnologiesmdu@gmail.com", "rgdplfzosznfipgc");
                        smtp.EnableSsl = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }

                //MimeMessage message = new MimeMessage();

                //MailboxAddress from = new MailboxAddress("User", "afarcabs123@gmail.com");
                //message.From.Add(from);

                //MailboxAddress to = new MailboxAddress("Admin", emailId);
                //message.To.Add(to);

                //message.Subject = "AFAR-Cabing Service | OTP to Verify Email";

                //BodyBuilder bodyBuilder = new BodyBuilder();
                //bodyBuilder.TextBody = "Hello Partner," + System.Environment.NewLine + System.Environment.NewLine + "AFAR CABS requires further verification" + System.Environment.NewLine + System.Environment.NewLine + "To complete the sign in, enter the verification code:" + System.Environment.NewLine + System.Environment.NewLine + otp.ToString() + System.Environment.NewLine + System.Environment.NewLine + "The verification code will be valid for 1 minute.Please do not share this code with anyone.";
                //message.Body = bodyBuilder.ToMessageBody();

                //SmtpClient client = new SmtpClient();
                //client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //client.Authenticate("afarcabs123@gmail.com", "pjsnthiwzelcgaua");

                //client.Send(message);
                //client.Disconnect(true);
                //client.Dispose();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public int SendMsg(string phonenum, int otp)
        {
            try
            {
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public List<LoginDetails> GetUpdatedProfile(int userId)
        {
            List<LoginDetails> UserResponse = new List<LoginDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetUpdateProfile", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = userId, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new LoginDetails()
                                {
                                    User_id = Convert.ToInt32(dr["user_id"]),
                                    Phone_num = dr["phone_num"].ToString(),
                                    Email_id = dr["email_id"].ToString(),
                                    User_type_flg = dr["user_type_flg"].ToString(),
                                    Name = dr["name"].ToString(),
                                    Referral_code = dr["referral_code"].ToString(),
                                    Wallet_Money = (decimal?)dr["wallet_balance"],
                                }).ToList();
            }

            return UserResponse;
        }

        public List<ResponseStatus> SaveLocation(LocationDetails locationDetails)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
                if (locationDetails != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "savelocation", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = locationDetails.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@loc_id", Value = locationDetails.Location_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@loc_type", Value = locationDetails.Location_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@loc_address", Value = locationDetails.Location_address, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@loc_street", Value = locationDetails.Location_street, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@pincode", Value = locationDetails.Pincode, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@loc_landmark", Value = locationDetails.Location_landmark, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@status", Value = locationDetails.Status, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.VarChar, Size = 100, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_taxi_usr_profile_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;
                        response.Add(respobj);
                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<LocationDetails> GetSavedLocation(int User_id, string Location_type)
        {
            List<LocationDetails> UserResponse = new List<LocationDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "Getsavelocation", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@loc_type", Value = Location_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_taxi_usr_profile_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new LocationDetails()
                                {
                                    User_id = Convert.ToInt32(dr["user_id"]),
                                    Location_id = Convert.ToInt32(dr["loc_id"]),
                                    Location_type = dr["loc_type"].ToString(),
                                    Location_address = dr["loc_address"].ToString(),
                                    Location_street = dr["loc_street"].ToString(),
                                    Pincode = dr["pincode"].ToString(),
                                    Location_landmark = dr["loc_landmark"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<ResponseStatus> SaveBookingDetails(BookingDetails bookingDetails)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
                if (bookingDetails != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "usrbooksearch", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = bookingDetails.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@fetch_id", Value = bookingDetails.User_track_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@from_location", Value = bookingDetails.From_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@to_location", Value = bookingDetails.To_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@from_latitude", Value = bookingDetails.From_latitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@from_longitude", Value = bookingDetails.From_longitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@to_latitude", Value = bookingDetails.To_latitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@to_longitude", Value = bookingDetails.To_longitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@fare_date", Value = bookingDetails.Fare_date, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@fare_type", Value = bookingDetails.Fare_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@Others_number", Value = bookingDetails.Others_number, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@vehicle_id", Value = bookingDetails.Vehicle_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@kms", Value = bookingDetails.Kms, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@cal_fare", Value = bookingDetails.Cal_fare, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@routed_driver_id", Value = bookingDetails.Routed_driver_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@fare_status", Value = bookingDetails.Fare_status, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@cancellation_reason", Value = bookingDetails.cancellation_reason, SqlDbType = SqlDbType.VarChar, Size = 1000, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@rating", Value = bookingDetails.rating, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@feedback", Value = bookingDetails.feedback, SqlDbType = SqlDbType.VarChar, Size = 1000, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.VarChar, Size = 100, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_taxi_usr_booking_search_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;
                        response.Add(respobj);
                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<DriverNotification> GetDriverNotificationDetails()
        {
            List<DriverNotification> UserResponse = new List<DriverNotification>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetDrivernotification", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new DriverNotification()
                                {
                                    Name = dr["name"].ToString(),
                                    Gender = dr["gender"].ToString(),
                                    Phone_num = dr["phone_num"].ToString(),
                                    Email_id = dr["email_id"].ToString(),
                                    Address = dr["usr_addr"].ToString(),
                                    Date_of_birth = dr["date_of_birth"].ToString(),
                                    Vehicle_type_id = dr["vehicle_type_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["vehicle_type_id"]),
                                    Vehicle_type_name = dr["vehicle_type_name"].ToString(),
                                    License_no = dr["license_no"].ToString(),
                                    License_plate_no = dr["license_plate_no"].ToString(),
                                    Insurance_no = dr["insurance_no"].ToString(),
                                    Aadhar_no = dr["aadhar_no"].ToString(),
                                    Usr_state = dr["usr_state"].ToString(),
                                    Usr_district = dr["usr_district"].ToString(),
                                    Country = dr["usr_country"].ToString(),
                                    Pincode = dr["pincode"].ToString(),
                                    Usr_img_file_location = dr["usr_img_file_location"].ToString(),
                                    Usr_img_file_name = dr["usr_img_file_name"].ToString(),
                                    User_address = dr["usr_addr"].ToString(),
                                    En_flag = dr["en_flg"].ToString(),
                                    Aadhar_file_name = dr["aadhar_file_name"].ToString(),
                                    Aadhar_file_location = dr["aadhar_file_location"].ToString(),
                                    Drv_pan_no = dr["drv_pan_no"].ToString(),
                                    Panno_file_name = dr["panno_file_name"].ToString(),
                                    Panno_file_location = dr["panno_file_location"].ToString(),
                                    Licno_file_name = dr["licno_file_name"].ToString(),
                                    Licno_file_location = dr["licno_file_location"].ToString(),
                                    insno_file_name = dr["insno_file_name"].ToString(),
                                    Insno_file_location = dr["insno_file_location"].ToString(),
                                    Plateno_file_name = dr["plateno_file_name"].ToString(),
                                    Plateno_file_location = dr["plateno_file_location"].ToString(),
                                    Doc_file_name = dr["doc_file_name"].ToString(),
                                    Doc_file_location = dr["doc_file_location"].ToString(),
                                    User_id = dr["user_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["user_id"]),
                                    First_Name = dr["first_name"].ToString(),
                                    Last_Name = dr["last_name"].ToString(),
                                    District_id = dr["district_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["district_id"]),
                                    Id_proof_name = dr["id_proof_name"].ToString(),
                                    Comments = dr["Comments"].ToString(),
                                    Notification_token = dr["notification_token"].ToString(),
                                    Token_msg = dr["token_msg"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<GetDriverPaymentDetails> GetDriverPaymentDetails(int User_Id)
        {
            List<GetDriverPaymentDetails> UserResponse = new List<GetDriverPaymentDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetDriverInitPayment", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = User_Id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new GetDriverPaymentDetails()
                                {
                                    User_Id = dr["user_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["user_id"]),
                                    First_Name = dr["first_name"].ToString(),
                                    Last_Name = dr["last_name"].ToString(),
                                    Email_Id = dr["email_id"].ToString(),
                                    Phone_Number = dr["phone_num"].ToString(),
                                    Bank_Name = dr["Bank_name"].ToString(),
                                    Branch_Name = dr["branch_name"].ToString(),
                                    Ifsc_Code = dr["ifsc_code"].ToString(),
                                    Bank_Img_File_Name = dr["bank_img_file_name"].ToString(),
                                    Bank_Img_File_Location = dr["bank_img_file_location"].ToString(),
                                    Account_Number = dr["Account_number"].ToString(),
                                    Bank_mobile_num = dr["bank_mobile_num"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<ExpiredVehicleInsurance> GetExpiredDrvLicense(ExpiredVehicleDetails expiredVehicleDetails)
        {
            List<ExpiredVehicleInsurance> expiredVehicleInsuranceDetails = new List<ExpiredVehicleInsurance>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparams = new List<DbParameter>();
            dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = expiredVehicleDetails.Userid, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@phone_num", Value = expiredVehicleDetails.Phonenum, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@email_id", Value = expiredVehicleDetails.Emailid, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@user_password", Value = expiredVehicleDetails.Userpassword, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@vehicle_license_no", Value = expiredVehicleDetails.VehicleLicenseNo, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@driver_name", Value = expiredVehicleDetails.DriverName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = expiredVehicleDetails.QueryName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.VarChar, Size = 500, Direction = ParameterDirection.Output });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparams, SQL_Helper.ExecutionType.Procedure);
            if (dt != null && dt.Rows.Count > 0)
            {
                expiredVehicleInsuranceDetails = (from DataRow dr in dt.Rows
                                                  select new ExpiredVehicleInsurance()
                                                  {
                                                      User_id = Convert.ToInt32(dr["user_id"]),
                                                      First_name = dr["first_name"].ToString(),
                                                      Last_name = dr["last_name"].ToString(),
                                                      Email_id = dr["email_id"].ToString(),
                                                      Phone_num = dr["phone_num"].ToString(),
                                                      Notification_token = dr["notification_token"].ToString(),
                                                      License_plate_no = dr["license_plate_no"].ToString(),
                                                      Expiry_date = dr["Expiry_date"].ToString(),

                                                      Msg = dr["msg"].ToString(),
                                                      flag = dr["flag"].ToString(),
                                                  }).ToList();
            }

            return expiredVehicleInsuranceDetails;
        }

        public List<ResponseStatus> AddReferralDetails(ReferralDetails ReferralDetails)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
                if (ReferralDetails != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "AddReferral", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@Bank_mobile_num", Value = ReferralDetails.Bank_mobile_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = ReferralDetails.user_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });
                    dbparams.Add(new SqlParameter { ParameterName = "@referral_code", Value = ReferralDetails.referral_code, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_taxi_driver_payment_Post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;

                        response.Add(respobj);


                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<ResponseStatus> SMSGatewayStatus(AddSMSGatewayStatus AddSMSGatewayStatus)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
                if (AddSMSGatewayStatus != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "smsgatewaystatus", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@phone_num", Value = AddSMSGatewayStatus.Phone_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@en_flg", Value = AddSMSGatewayStatus.En_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@Screen_type", Value = AddSMSGatewayStatus.Screen_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@Template_Id", Value = AddSMSGatewayStatus.Template_Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = AddSMSGatewayStatus.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;

                        response.Add(respobj);


                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<ResponseStatus> UserPwdUpdate(PwdUpdate PwdUpdate)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
                if (PwdUpdate != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                    DataTable dt = new DataTable();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "userupdatePwd", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@phone_num", Value = PwdUpdate.Phone_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_password", Value = PwdUpdate.User_password, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_type_flg", Value = PwdUpdate.User_type_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;

                        response.Add(respobj);


                    }
                }
                return response;
            }
            catch (Exception)
            {
                return response;
            }
        }

        public List<LoginDetails> GetUserByPhoneOrEmail(string PhoneNumber)
        {
            List<LoginDetails> UserResponse = new List<LoginDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetUserByPhoneOrEmail", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@phone_num", Value = PhoneNumber, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_reg_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new LoginDetails()
                                {
                                    User_id = Convert.ToInt32(dr["user_id"]),
                                    Name = dr["name"].ToString(),
                                    Phone_num = dr["phone_num"].ToString(),
                                    Email_id = dr["email_id"].ToString(),
                                    User_type_flg = dr["user_type_flg"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<ConfigSettings> GetMasterSettings(string settingsName)
        {
            List<ConfigSettings> UserResponse = new List<ConfigSettings>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrsettings", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@settings_name", Value = settingsName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new ConfigSettings()
                                {
                                    Settings_id = Convert.ToInt32(dr["settings_id"]),
                                    Settings_name = dr["settings_name"].ToString(),
                                    Settings_value = dr["settings_value"].ToString(),
                                    Setting_desc = dr["setting_desc"].ToString(),
                                    En_flg = dr["en_flg"].ToString(),
                                    Type = dr["Type"].ToString(),
                                    Days = dr["Days"].ToString(),
                                    file_location = dr["file_location"].ToString(),
                                    file_name = dr["file_name"].ToString(),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<ReferralDetails> GetChkReferralCode(string ReferralCode, string UserTypeFlg)
        {
            List<ReferralDetails> UserResponse = new List<ReferralDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetChkReferralCode", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@referral_code", Value = ReferralCode, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_type_flg", Value = UserTypeFlg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_book_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                UserResponse = (from DataRow dr in dt.Rows
                                select new ReferralDetails()
                                {
                                    user_id = Convert.ToInt32(dr["user_id"]),
                                }).ToList();
            }

            return UserResponse;
        }

        public List<FareCalculations> GetFareCalculations(int userid, string frmloc, string toloc,
            string frmlat, string frmlong, string tolat, string tolong, string kms, string traveltime)
        {
            List<FareCalculations> FareResponse = new List<FareCalculations>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsFareInfo = new List<DbParameter>();
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetAllVehiclesFare", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = userid, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@from_location", Value = frmloc, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@to_location", Value = toloc, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@from_latitude", Value = frmlat, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@from_longitude", Value = frmlong, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@to_latitude", Value = tolat, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@to_longitude", Value = tolong, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@kms", Value = kms, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
            dbparamsFareInfo.Add(new SqlParameter { ParameterName = "@fare_date", Value = traveltime, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_taxi_usr_allvhcl_fare_cal", dbparamsFareInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                FareResponse = (from DataRow dr in dt.Rows
                                select new FareCalculations()
                                {
                                    User_id = Convert.ToInt32(dr["user_id"]),//dr["user_id"] == System.DBNull.Value ? null : Convert.ToInt32(dr["user_id"]),
                                    Kms = (decimal?)dr["kms"],
                                    Cal_fare = (decimal?)dr["cal_fare"],
                                    Vehicle_id = (int?)dr["vehicle_id"],
                                    Vehicle_name = dr["vehicle_name"].ToString(),
                                    file_location = dr["file_location"].ToString(),
                                    //Peak_flg = dr["peak_flg"].ToString(),
                                    fare_date_time = dr["fare_date_time"].ToString(),
                                }).ToList();
            }

            return FareResponse;
        }

        public List<UserBookSearchModel> PostDriversCurrLocation(DriversCurrLocation DriversCurrLocation)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            List<UserBookSearchModel> userBookSearch = new List<UserBookSearchModel>();
            DataTable dt = new DataTable();
            try
            {
                if (DriversCurrLocation != null)
                {
                    Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

                    List<DbParameter> dbparams = new List<DbParameter>();
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@driver_id", Value = DriversCurrLocation.Driver_Id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@latitude", Value = DriversCurrLocation.Latitude, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@longitude", Value = DriversCurrLocation.Longitude, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@loc_name", Value = DriversCurrLocation.Location_Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@rec_created_userid", Value = DriversCurrLocation.Created_UserId, SqlDbType = SqlDbType.Int, Size = 1000, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_taxi_driver_location_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();
                        respobj.Error_desc = spOut;

                        response.Add(respobj);
                    }
                    
                    List<DbParameter> dbparamsbookInfo = new List<DbParameter>();
                    dbparamsbookInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetChkNearUsers", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparamsbookInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = DriversCurrLocation.Driver_Id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparamsbookInfo.Add(new SqlParameter { ParameterName = "@latitude", Value = DriversCurrLocation.Latitude, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparamsbookInfo.Add(new SqlParameter { ParameterName = "@longitude", Value = DriversCurrLocation.Longitude, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
                    dbparamsbookInfo.Add(new SqlParameter { ParameterName = "@loc_name", Value = DriversCurrLocation.Location_Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_book_get", dbparamsbookInfo, SQL_Helper.ExecutionType.Procedure);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        userBookSearch = (from DataRow dr in dt.Rows
                                            select new UserBookSearchModel()
                                            {
                                                User_Id = Convert.ToInt32(dr["user_id"]),
                                                Search_Id = Convert.ToInt32(dr["Search_Id"]),
                                                Name = dr["User_Name"].ToString(),
                                                Phone_Num = dr["User_Phone_Num"].ToString(),
                                                OTP = Convert.ToInt32(dr["otp"]),
                                                Fare_Date = dr["fare_date"].ToString(),
                                                From_Latitude = dr["User_Start_Lat"].ToString(),
                                                From_Longitude = dr["User_Start_Long"].ToString(),
                                                To_Latitude = dr["User_End_Lat"].ToString(),
                                                To_Longitude = dr["User_End_Long"].ToString(),
                                                Kms = Convert.ToDecimal(dr["Usr_Req_Kms"]),
                                                Cal_Fare = Convert.ToDecimal(dr["Usr_Req_Fare"]),
                                                //////Gender = dr["gender"].ToString(),
                                                From_Location = dr["from_location"].ToString(),
                                                To_Location = dr["To_Location"].ToString(),
                                                status_flg = dr["status_flg"].ToString(),
                                                Fare_Type = dr["fare_type"].ToString(),
                                                Fare_Status = dr["fare_status"].ToString(),
                                                Others_Number = dr["others_num"].ToString(),
                                                Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                                                Delivery = dr["Delivery"].ToString(),
                                                //////Comments = dr["comments"].ToString(),
                                                //////Routed_Driver_Id = Convert.ToInt32(dr["routed_driver_id"]),
                                                diff_distance_fromur_loc = Convert.ToDecimal(dr["diff_distance_fromur_loc"])
                                            }).ToList();
                    }
                }
                  return userBookSearch;
            }
            catch (Exception)
            {
                return userBookSearch;
            }
        }

        public List<ReferralDetails> GetDriversNearBy2Kms(int otp, decimal Latitude, decimal Longitude, decimal To_Latitude, decimal To_Longitude, decimal Fare, decimal Fare_Requested_In_Kms, string Location_Name, int user_id, string status_flg, int Vehicle_id, string fare_type, string others_num, string from_location, string to_location)
        {
            List<ReferralDetails> DriversNearBy2Kms = new List<ReferralDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetChkNearDrivers", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@Latitude", Value = Latitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@longitude", Value = Longitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@To_Latitude", Value = To_Latitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@To_longitude", Value = To_Longitude, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@fare", Value = Fare, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_req_distance", Value = Fare_Requested_In_Kms, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@user_id", Value = user_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@loc_name", Value = Location_Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@otp", Value = otp, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@status_flg", Value = status_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@vehicle_id", Value = Vehicle_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@fare_type", Value = fare_type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@others_num", Value = others_num, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@From_Location", Value = from_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@To_Location", Value = to_location, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });


            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_book_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                DriversNearBy2Kms = (from DataRow dr in dt.Rows
                                     select new ReferralDetails()
                                     {
                                         user_id = Convert.ToInt32(dr["user_id"]),
                                         driver_id = Convert.ToInt32(dr["driver_id"]),
                                         Driver_Latitude = Convert.ToDecimal(dr["Driver_Latitude"]),
                                         Driver_Phone_No = dr["Driver_Phone_Num"].ToString(),
                                         Driver_Longitude = Convert.ToDecimal(dr["Driver_Longitude"]),
                                         Driver_Location_Name = dr["Driver_loc_name"].ToString(),
                                         User_Name = dr["User_Name"].ToString(),
                                         User_Phone_Num = dr["User_Phone_Num"].ToString(),
                                         User_Location_Name = dr["User_Location"].ToString(),
                                         User_Latitude = Convert.ToDecimal(dr["User_Latitude"]),
                                         User_Longitude = Convert.ToDecimal(dr["User_Longitude"]),
                                         Fare = Convert.ToDecimal(dr["Req_Tot_Fare"]),
                                         Fare_Requested_In_Kms = Convert.ToDecimal(dr["Approx_Req_Distance"]),
                                         OTP = Convert.ToInt32(dr["OTP"]),
                                         Driver_Name = dr["Driver_Name"].ToString(),
                                         Driver_Photo = dr["Driver_photo"].ToString(),
                                         Driver_Rating = Convert.ToDecimal(dr["Rating"]),
                                         Vehicle_No = dr["Vehicle_No"].ToString(),
                                         Vehicle_img = dr["Vehicle_Image"].ToString(),
                                         Vehicle_Name = dr["Vehicle_Name"].ToString(),
                                         User_Track_Id = Convert.ToInt32(dr["User_Track_Id"]),
                                         Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                                         Payment_Type = dr["Payment_Type"].ToString(),
                                         Final_Status_Flg = dr["Final_Status_Flg"].ToString(),
                                     }).ToList();
            }

            return DriversNearBy2Kms;
        }

        

        public List<UserDetails> GetUsersForPushNotifications(string En_flag, string User_type_flg)
        {
            List<UserDetails> PushNotifications = new List<UserDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetUsersorDrivers", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@loc_type", Value = User_type_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });


            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_taxi_usr_profile_get", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

            if (dt != null && dt.Rows.Count > 0)
            {
                PushNotifications = (from DataRow dr in dt.Rows
                                     select new UserDetails()
                                     {
                                         User_id = Convert.ToInt32(dr["user_id"]),
                                         First_name = dr["User_Name"].ToString(),
                                         Phone_number = dr["User_Phone_Num"].ToString(),
                                         Email_id = dr["email_id"].ToString(),
                                         User_type_flg = dr["user_type_flg"].ToString(),
                                         Referral_code = dr["referral_code"].ToString(),
                                         Image_data = dr["image_path"].ToString(),
                                         message = dr["message"].ToString(),
                                     }).ToList();
            }

            return PushNotifications;
        }

        public List<CityRangeDetails> GetCityRangeDetails(string city_name)
        {
            List<CityRangeDetails> cityRangeDetails = new List<CityRangeDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparams = new List<DbParameter>();
            dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "mstrcityrange", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@settings_name", Value = city_name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_mstr", dbparams, SQL_Helper.ExecutionType.Procedure);
            if (dt != null && dt.Rows.Count > 0)
            {
                cityRangeDetails = (from DataRow dr in dt.Rows
                                    select new CityRangeDetails()
                                    {
                                        city_name = dr["CITY_NAME"].ToString(),
                                        city_id = Convert.ToInt32(dr["CITY_ID"]),
                                        city_latitude = dr["CENTRE_LATITUDE"].ToString(),
                                        city_longitude = dr["CENTRE_LONGITUDE"].ToString(),
                                        city_range = dr["CITY_TOTAL_RADIUS"].ToString(),
                                    }).ToList();
            }

            return cityRangeDetails;
        }

        public List<BookingDetails> GetOverallUserRides(int user_id, string status_flg)
        {
            List<BookingDetails> overallUserRides = new List<BookingDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparams = new List<DbParameter>();
            dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetUserRides", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = user_id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dbparams.Add(new SqlParameter { ParameterName = "@status_flg", Value = status_flg, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            dt = SQL_Helper.ExecuteSelect<SqlConnection>("usp_mileapp_usr_book_get", dbparams, SQL_Helper.ExecutionType.Procedure);
            if (dt != null && dt.Rows.Count > 0)
            {
                overallUserRides = (from DataRow dr in dt.Rows
                                    select new BookingDetails()
                                    {
                                        User_id = Convert.ToInt32(dr["user_id"]),
                                        Routed_driver_id = Convert.ToInt32(dr["routed_driver_id"]),
                                        From_location = dr["from_location"].ToString(),
                                        To_location = dr["to_location"].ToString(),
                                        From_latitude = dr["latitude"].ToString(),
                                        From_longitude = dr["longitude"].ToString(),
                                        To_latitude = dr["To_latitude"].ToString(),
                                        To_longitude = dr["To_longitude"].ToString(),
                                        Fare_date = dr["fare_date"].ToString(),
                                        cancellation_reason = dr["cancellation_reason"].ToString(),
                                        Kms = (decimal?)dr["travelled_kms"],
                                        Cal_fare = (decimal?)dr["travelled_cal_fare"],
                                        Vehicle_id = Convert.ToInt32(dr["Vehicle_Id"]),
                                        Vehicle_type_name = dr["Vehicle_Type"].ToString(),
                                        OrderId = dr["OrderId"].ToString(),
                                        driver_start_time = dr["driver_start_time"].ToString(),
                                        driver_end_time = dr["driver_end_time"].ToString(),
                                        delivery = dr["delivery"].ToString(),
                                        feedback = dr["feedback"].ToString(),
                                        rating = (decimal?)dr["rating"],
                                    }).ToList();
            }

            return overallUserRides;
        }

    }
}

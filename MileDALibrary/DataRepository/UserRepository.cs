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
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
                                      Country_id = Convert.ToInt32(dr["country_id"])
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
                    dbparams.Add(new SqlParameter { ParameterName = "@screen_type", Value = updateProfile.Screen_type, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@template_id", Value = updateProfile.Referral_Code, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
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
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("User", "afarcabs123@gmail.com");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("Admin", emailId);
                message.To.Add(to);

                message.Subject = "AFAR-Cabing Service | OTP to Verify Email";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Hello Partner," + System.Environment.NewLine + System.Environment.NewLine + "AFAR CABS requires further verification" + System.Environment.NewLine + System.Environment.NewLine + "To complete the sign in, enter the verification code:" + System.Environment.NewLine + System.Environment.NewLine + otp.ToString() + System.Environment.NewLine + System.Environment.NewLine + "The verification code will be valid for 1 minute.Please do not share this code with anyone.";
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("afarcabs123@gmail.com", "pjsnthiwzelcgaua");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();

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
            catch(Exception)
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

        public List<LocationDetails> GetSavedLocation()
        {
            List<LocationDetails> UserResponse = new List<LocationDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "Getsavelocation", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
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
                    dbparams.Add(new SqlParameter { ParameterName = "@query_name", Value = "usrbook", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = bookingDetails.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@user_track_id", Value = bookingDetails.User_track_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
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
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.VarChar, Size = 100, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_taxi_usr_booking_post", dbparams, SQL_Helper.ExecutionType.Procedure);

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


    }
}

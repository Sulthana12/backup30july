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

        public List<ResponseStatus> UpdateProfileDetails(UpdateProfile updateProfile)
        {
            int insertRowsCount = 0;
            List<ResponseStatus> response = new List<ResponseStatus>();
            try
            {
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
                    dbparams.Add(new SqlParameter { ParameterName = "@user_id", Value = updateProfile.User_id, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
                    dbparams.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                    result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparams, SQL_Helper.ExecutionType.Procedure);

                    insertRowsCount = insertRowsCount + result["RowsAffected"];

                    string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                    if (!string.IsNullOrEmpty(spOut))
                    {
                        ResponseStatus respobj = new ResponseStatus();

                        //string[] a = spOut.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        //foreach (var keyvaluepair in spOut.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        //{
                        //string[] splitteddata = keyvaluepair.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);


                        //if (splitteddata[0].Trim().Equals("error_desc"))
                        //{
                        respobj.Error_desc = spOut;

                        //}
                        //}

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

        public List<DriverDetails> GetDriverDetails()
        {
            List<DriverDetails> UserResponse = new List<DriverDetails>();
            DataTable dt = new DataTable();
            List<DbParameter> dbparamsUserInfo = new List<DbParameter>();
            dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@query_name", Value = "GetDriverDetails", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
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
                                    Vehicle_type_id = Convert.ToInt32(dr["vehicle_type_id"]),
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
                                    User_id = Convert.ToInt32(dr["user_id"]),
                                    First_Name = dr["first_name"].ToString(),
                                    Last_Name = dr["last_name"].ToString(),
                                    District_id = Convert.ToInt32(dr["district_id"]),
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


                    string imagedata = UserRepository.ScaleImage(userDetails.Image_data, 140, 140);

                    userDetails.Image_data = string.Empty;
                    userDetails.Image_data = imagedata;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" +"image"+ DateTime.Now.ToString("dd-MM-yyyy") + ".jpg";
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


                    string docData = UserRepository.ScaleImage(userDetails.Doc_data, 140, 140);

                    userDetails.Doc_data = string.Empty;
                    userDetails.Doc_data = docData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "doc" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";
                    blobEntity.ByteArray = userDetails.Doc_data;

                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("afar-blob");

                    string blobPath = blobEntity.DirectoryName + "/" + blobEntity.FolderName;

                    BlobClient blobClient = containerClient.GetBlobClient(blobPath);

                    Byte[] bytes1 = Convert.FromBase64String(blobEntity.ByteArray);
                    Stream stream = new MemoryStream(bytes1);

                    var response = await blobClient.UploadAsync(stream, true);

                    userDetails.Doc_file_location1 = this.blobconfig.Value.UserProfilePhoto;
                    userDetails.Doc_file_name1 = blobEntity.FolderName;

                }

                if (!string.IsNullOrEmpty(userDetails.Aadhar_data))
                {


                    string aadharData = UserRepository.ScaleImage(userDetails.Aadhar_data, 140, 140);

                    userDetails.Aadhar_data = string.Empty;
                    userDetails.Aadhar_data = aadharData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "aadhar" + DateTime.Now.ToString("dd-MM-yyyy") + ".jpg";
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


                    string panData = UserRepository.ScaleImage(userDetails.Pan_data, 140, 140);

                    userDetails.Pan_data = string.Empty;
                    userDetails.Pan_data = panData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "pan" + DateTime.Now.ToString("dd-MM-yyyy") + ".jpg";
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


                    string licenseData = UserRepository.ScaleImage(userDetails.License_data, 140, 140);

                    userDetails.License_data = string.Empty;
                    userDetails.License_data = licenseData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "license" + DateTime.Now.ToString("dd-MM-yyyy") + ".jpg";
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


                    string insuranceData = UserRepository.ScaleImage(userDetails.Insurance_data, 140, 140);

                    userDetails.Insurance_data = string.Empty;
                    userDetails.Insurance_data = insuranceData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "insurance" + DateTime.Now.ToString("dd-MM-yyyy") + ".jpg";
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


                    string plateNoData = UserRepository.ScaleImage(userDetails.PlateNo_data, 140, 140);

                    userDetails.PlateNo_data = string.Empty;
                    userDetails.PlateNo_data = plateNoData;

                    BlobEntity blobEntity = new BlobEntity();
                    blobEntity.DirectoryName = "Profile";
                    blobEntity.FolderName = userDetails.First_name + "-" + userDetails.User_id + "-" + "plateno" + DateTime.Now.ToString("dd-MM-yyyy") + ".jpg";
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
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@doc_file_name1", Value = String.IsNullOrEmpty(userDetails.Doc_file_name1) ? DBNull.Value : (object)userDetails.Doc_file_name1, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@doc_file_location1", Value = String.IsNullOrEmpty(userDetails.Doc_file_location1) ? DBNull.Value : (object)userDetails.Doc_file_location1, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
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
                dbparamsUserInfo.Add(new SqlParameter { ParameterName = "@response_status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output });

                result = SQL_Helper.ExecuteNonQuery<SqlConnection>("usp_mileapp_usr_reg_post", dbparamsUserInfo, SQL_Helper.ExecutionType.Procedure);

                insertRowsCount = insertRowsCount + result["RowsAffected"];

                string spOut = DBNull.Value.Equals(result["@response_status"]) ? "" : result["@response_status"];
                if (!string.IsNullOrEmpty(spOut))
                {
                    ResponseStatus respobj = new ResponseStatus();

                    //string[] a = spOut.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    //foreach (var keyvaluepair in spOut.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    //{
                    //string[] splitteddata = keyvaluepair.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);


                    //if (splitteddata[0].Trim().Equals("error_desc"))
                    //{
                    respobj.Error_desc = spOut;

                    //}
                    //}

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

                MailboxAddress from = new MailboxAddress("User","afarcabs@gmail.com");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("Admin", emailId);
                message.To.Add(to);

                message.Subject = "Afar Cabs - OTP";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Hi User," + System.Environment.NewLine + System.Environment.NewLine + "You can use the following OTP in Afar Cabs App" + System.Environment.NewLine + otp.ToString() + System.Environment.NewLine + "Thanks," + System.Environment.NewLine + "Afar Cabs";
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("afarcabs@gmail.com","oifikzxjevrqahoi");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();

                return 1;
            }
            catch (Exception ex)
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
                                }).ToList();
            }

            return UserResponse;
        }
    }
}

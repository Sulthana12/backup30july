using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MileDALibrary.Interfaces;
using MileDALibrary.Helper;
using MileDALibrary.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace MileDALibrary.DataRepository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly IOptions<DBSettings> options;
        string istStrDate = "select CAST(DATEADD(HOUR, 5, DATEADD(MINUTE, 30, GETUTCDATE())) as DATE)";
        private string istDate = "";

        public UserRepository(IConfiguration config, IOptions<DBSettings> options) : base(config)
        {
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
    }
}

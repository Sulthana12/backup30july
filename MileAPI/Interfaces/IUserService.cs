using MileDALibrary.Models;

namespace MileAPI.Interfaces
{
    public interface IUserService
    {
        List<LoginDetails> GetUserInformation(string PhoneNumber, string Password);

        List<CountryDetails> GetCountryDetails();

        List<StateDetails> GetStateDetails();

        List<DistrictDetails> GetDistrictDetails(int stateId, int countryId);

        List<VehicleDetails> GetVehicleDetails();

        List<GenderDetails> GetGenderDetails(string settingsName);

        Task<List<ResponseStatus>> UpdateProfileDetails(UpdateProfile updateProfile);

        List<DriverDetails> GetDriverDetails(string phoneNumber);

        Task<List<ResponseStatus>> SaveUserDetails(UserDetails userDetails);

        int SendEmail(string emailId, int otp);

        List<LoginDetails> GetUpdatedProfile(int userId);

        List<ResponseStatus> SaveLocation(LocationDetails locationDetails);

        List<LocationDetails> GetSavedLocation(int User_id, string Location_type);

        List<ResponseStatus> SaveBookingDetails(BookingDetails bookingDetails);

        List<DriverNotification> GetDriverNotificationDetails();

        Task<List<ResponseStatus>> DriverPaymentDetails(DriverPaymentDetails driverPaymentDetails);

        List<GetDriverPaymentDetails> GetDriverPaymentDetails(int User_Id);

        List<ExpiredVehicleInsurance> GetExpiredDrvLicense(ExpiredVehicleDetails expiredVehicleDetails);

        List<ResponseStatus> AddReferralDetails(ReferralDetails ReferralDetails);
        List<ResponseStatus> DriverRegPaymentStatusDetails(DriverRegPaymentStatus DriverRegPaymentStatus);

        List<ResponseStatus> SMSGatewayStatus(AddSMSGatewayStatus AddSMSGatewayStatus);

        List<ResponseStatus> UserPwdUpdate(PwdUpdate PwdUpdate);

        List<LoginDetails> GetUserByPhoneOrEmail(string PhoneNumber);
        List<ConfigSettings> GetMasterSettings(string settingsName);
        List<ReferralDetails> GetChkReferralCode(string ReferralCode, string UserTypeFlg);
        List<FareCalculations> GetFareCalculations(int userid, string frmloc, string toloc,
            string frmlat, string frmlong, string tolat, string tolong, string kms, string traveltime);
        List<UserBookSearchModel> PostDriversCurrLocation(DriversCurrLocation DriversCurrLocation);
        List<ReferralDetails> GetDriversNearBy2Kms(int otp, decimal Latitude, decimal Longitude, decimal To_Latitude, decimal To_Longitude, decimal Fare, decimal Fare_Requested_In_Kms, string Location_Name, int user_id, string status_flg, int Vehicle_id, string fare_type, string others_num, string from_location, string to_location);
        List<CityRangeDetails> GetCityRangeDetails(string city_name);
        List<UserDetails> GetUsersForPushNotifications(string En_flag, string User_type_flg);
        List<BookingDetails> GetOverallUserRides(int user_id, string status_flg, int user_track_id);
    }
}

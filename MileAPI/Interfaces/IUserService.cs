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

        List<ResponseStatus> UpdateProfileDetails(UpdateProfile updateProfile);

        List<DriverDetails> GetDriverDetails(string phoneNumber);

        Task<List<ResponseStatus>> SaveUserDetails(UserDetails userDetails);

        int SendEmail(string emailId, int otp);

        List<LoginDetails> GetUpdatedProfile(int userId);

        List<ResponseStatus> SaveLocation(LocationDetails locationDetails);

        List<LocationDetails> GetSavedLocation();

        List<ResponseStatus> SaveBookingDetails(BookingDetails bookingDetails);

        List<DriverNotification> GetDriverNotificationDetails();

        Task<List<ResponseStatus>> DriverPaymentDetails(DriverPaymentDetails driverPaymentDetails);

        List<GetDriverPaymentDetails> GetDriverPaymentDetails(int User_Id);

        List<ExpiredVehicleInsurance> GetExpiredDrvLicense(ExpiredVehicleDetails expiredVehicleDetails);

        List<ResponseStatus> AddReferralDetails(ReferralDetails ReferralDetails);
        List<ResponseStatus> DriverRegPaymentStatusDetails(DriverRegPaymentStatus DriverRegPaymentStatus);

    }
}

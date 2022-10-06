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

        List<DriverDetails> GetDriverDetails(string phoneNumber, string vehicleLicenseNumber, string driverName);

        Task<UserDetails> SaveUserDetails(UserDetails userDetails);
    }
}

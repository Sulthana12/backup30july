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

        List<DriverDetails> GetDriverDetails();

        Task<List<ResponseStatus>> SaveUserDetails(UserDetails userDetails);

        int SendEmail(string emailId, int otp);

        List<LoginDetails> GetUpdatedProfile(int userId);
    }
}

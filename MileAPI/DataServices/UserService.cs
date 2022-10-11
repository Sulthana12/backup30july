using MileAPI.Interfaces;
using MileDALibrary.Interfaces;
using MileDALibrary.Models;

namespace MileAPI.DataServices
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<LoginDetails> GetUserInformation(string PhoneNumber, string Password)
        {
            return _userRepository.GetUserInformation(PhoneNumber, Password);
        }

        public List<CountryDetails> GetCountryDetails()
        {
            return _userRepository.GetCountryDetails();
        }

        public List<StateDetails> GetStateDetails()
        {
            return _userRepository.GetStateDetails();
        }

        public List<DistrictDetails> GetDistrictDetails(int stateId, int countryId)
        {
            return _userRepository.GetDistrictDetails(stateId, countryId);
        }

        public List<VehicleDetails> GetVehicleDetails()
        {
            return _userRepository.GetVehicleDetails();
        }

        public List<GenderDetails> GetGenderDetails(string settingsName)
        {
            return _userRepository.GetGenderDetails(settingsName);
        }

        public List<ResponseStatus> UpdateProfileDetails(UpdateProfile updateProfile)
        {
            return _userRepository.UpdateProfileDetails(updateProfile);
        }

        public List<DriverDetails> GetDriverDetails()
        {
            return _userRepository.GetDriverDetails();
        }

        public Task<List<ResponseStatus>> SaveUserDetails(UserDetails userDetails)
        {
            return _userRepository.SaveUserDetails(userDetails);
        }
    }
}

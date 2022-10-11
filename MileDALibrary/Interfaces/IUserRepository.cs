using MileDALibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Interfaces
{
    public interface IUserRepository
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
    }
}

using MileDALibrary.Models;

namespace MileAPI.Interfaces
{
    public interface IUserService
    {
        List<LoginDetails> GetUserInformation(string PhoneNumber, string Password);
    }
}

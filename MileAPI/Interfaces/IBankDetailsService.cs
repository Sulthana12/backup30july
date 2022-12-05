using MileDALibrary.Models;

namespace MileAPI.Interfaces
{
    public interface IBankDetailsService
    {
        Task<BankDetails> GetBankDetails(string ifscCode);
    }
}

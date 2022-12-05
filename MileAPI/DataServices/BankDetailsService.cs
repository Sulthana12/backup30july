using MileAPI.Interfaces;
using MileDALibrary.Interfaces;
using MileDALibrary.Models;

namespace MileAPI.DataServices
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly IBankDetailsRepository _bankDetailsRepository;

        public BankDetailsService(IBankDetailsRepository bankDetailsRepository)
        {
            _bankDetailsRepository = bankDetailsRepository;
        }
        public async Task<BankDetails> GetBankDetails(string ifscCode)
        {
            return await _bankDetailsRepository.GetBankDetails(ifscCode);
        }
    }
}

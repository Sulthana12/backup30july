using MileDALibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Interfaces
{
    public interface IBankDetailsRepository
    {
        Task<BankDetails> GetBankDetails(string ifscCode);
    }
}

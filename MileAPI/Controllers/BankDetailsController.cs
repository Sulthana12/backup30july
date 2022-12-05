using Microsoft.AspNetCore.Mvc;
using MileAPI.Interfaces;
using MileDALibrary.Models;

namespace MileAPI.Controllers
{
    [Route("api/bankDetails")]
    [ApiController]
    public class BankDetailsController : Controller
    {
        private readonly IBankDetailsService _bankDetailsService;

        public BankDetailsController(IBankDetailsService bankDetailsService)
        {
            _bankDetailsService = bankDetailsService;
        }

        [HttpGet("GetBankDetails/{ifscCode}")]
        public async Task<IActionResult> GetBankDetails(string ifscCode)
        {
            try
            {
                BankDetails result = await _bankDetailsService.GetBankDetails(ifscCode);
                if (result == null)
                {
                    return Unauthorized("{\"status\": \" No Data\"}");
                }
                
                return Ok(result);
            }
            catch (Exception)
            {
                return Unauthorized("{\"status\": \"No Data\"}");
            }
        }
    }
}

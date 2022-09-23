using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MileAPI.Interfaces;
using MileDALibrary.Models;

namespace MileAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserDetails")]
        public IActionResult GetUserInformation(string PhoneNumber, string Password)
        {
            try
            {
                List<LoginDetails> result = _userService.GetUserInformation(PhoneNumber, Password);
                if (result == null)
                {
                    return Unauthorized("Authentication Failed");
                }
                if(result.Count == 0)
                {
                    return NotFound("Invalid Mobile No/Password");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("Not Found");
            }   
        }

        [HttpGet("GetCountryDetails")]
        public IActionResult GetCountryNames()
        {
            try
            {
                List<CountryDetails> result = _userService.GetCountryDetails();
                if (result == null)
                {
                    return Unauthorized("Authentication Failed");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("Not Found");
            }
        }

        [HttpGet("GetStateDetails")]
        public IActionResult GetStateDetails()
        {
            try
            {
                List<StateDetails> result = _userService.GetStateDetails();
                if (result == null)
                {
                    return Unauthorized("Authentication Failed");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("Not Found");
            }
        }
    }
}

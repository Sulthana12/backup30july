using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MileAPI.Interfaces;
using MileDALibrary.Models;

namespace MileAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserDetails")]
        //[Route("/")]
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
        //[Route("/")]
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
        //[Route("/")]
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

        [HttpGet("GetDistrictDetails")]
        //[Route("/")]
        public IActionResult GetDistrictDetails(int stateId, int countryId)
        {
            try
            {
                List<DistrictDetails> result = _userService.GetDistrictDetails(stateId, countryId);
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

        [HttpGet("GetVehicleDetails")]
        //[Route("/")]
        public IActionResult GetVehicleDetails()
        {
            try
            {
                List<VehicleDetails> result = _userService.GetVehicleDetails();
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

        [HttpGet("GetGenderDetails")]
        //[Route("/")]
        public IActionResult GetGenderDetails(string settingsName)
        {
            try
            {
                List<GenderDetails> result = _userService.GetGenderDetails(settingsName);
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

        [HttpPost("PostUpdatedProfileDetails")]
        public IActionResult UpdateProfileDetails([FromBody] UpdateProfile updateProfile)
        {
            int result = _userService.UpdateProfileDetails(updateProfile);
            if (result == 1)
            {
                return Ok(new { data = "Updated Successfully" });
            }
            else
            {
                return NotFound();
            }

        }
    }
}

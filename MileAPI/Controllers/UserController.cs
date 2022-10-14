using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MileAPI.Interfaces;
using MileDALibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if(result.Count == 0)
                {
                    return NotFound("{\"status\": \"Invalid Mobile No/Password\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
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
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
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
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
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
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
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
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
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
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("PostUpdatedProfile/SignUpDetails")]
        public IActionResult UpdateProfileDetails([FromBody] UpdateProfile updateProfile)
        {
            string flag;
            List<ResponseStatus> result = _userService.UpdateProfileDetails(updateProfile);
            if (!result.Any())
            {
                return NotFound("{\"status\": \"Failed\"}");
            }
            flag = result[0].Error_desc;
            if (flag.Contains("Success"))
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("GetDriverDetails")]
        //[Route("/")]
        public IActionResult GetDriverDetails()
        {
            try
            {
                List<DriverDetails> result = _userService.GetDriverDetails();
                if (result == null)
                {
                    return Unauthorized("{\"status\": \"Authentication Failed\"}");
                }
                if (result.Count == 0)
                {
                    return NotFound("{\"status\": \"No Data Found\"}");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Not Found\"}");
            }
        }

        [HttpPost("SaveUserDetails")]
        public async Task<IActionResult> SaveUserDetails([FromBody] UserDetails userDetails)
        {
            string flag;
            try
            {
                List<ResponseStatus> result = await _userService.SaveUserDetails(userDetails);

                if (!result.Any())
                {
                    return NotFound("{\"status\": \"Failed\"}");
                }
                flag = result[0].Error_desc;
                if (flag.Contains("Success"))
                {
                    return Ok(result);
                }

                return NotFound(result);

            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Insertion Failed\"}");
            }
        }

        [HttpGet("ValidateUser")]
        public async Task<IActionResult> ValidateUser(string phoneNumber)
        {
            try
            {
                const string filePath = "\r\n\r\n<div style=\"font-size:1.1em;color:black!important\">\r\n        <span>Hi,</span><br />\r\n        <br />\r\n        <span>Use the following one-time password (OTP) to sign in to your Bihan App account.</span><br />\r\n        <br />\r\n        <span style=\"font-weight:bold;font-size:1.8em!important;\">otpplaceholder</span><br />\r\n\r\n        <span>(The OTP will be valid for the next 5 minutes)</span><br />\r\n        <span>If you didn't initiate this action or if you think you received this email by mistake, please contact adidaassupport@bihan.com</span><br />\r\n        <br />\r\n       \r\n        <span>*This is an auto generated e-mail, therefore do not reply to this email.*</span><br />\r\n        <br />\r\n        \r\n        Regards,<br />\r\n        BIHAN TEAM\r\n    </div>\r\n\r\n\r\n\r\n\r\n\r\n\r\n";

                int otp = RandomNumberGenerator.GetInt32(100000, 999999);

                return Ok(otp);
            }
            catch (Exception)
            {
                return NotFound("{\"status\": \"Failed To Send SMS\"}");
            }
        }
    }
}

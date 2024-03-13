using IdentityApi.Models;
using IdentityApi.Models.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisteredUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.SignUp(user);
                if (result.Succeeded)
                {
                    return Ok("Registration successful");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("verifyemail")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid Email Or Token.");
            }

            var result = await _accountRepository.VerifyEmail(email, token);

            if (result)
            {
                return Ok("Email verified");
            }
            else
            {
                return BadRequest("An error occurred.");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string userName, string Password)
        {
            var login = await _accountRepository.Login(userName, Password);

            if (login.IsNotAllowed)
            {
                return BadRequest("Confirm email before login.");
            }
            if (!login.Succeeded)
            {
                return BadRequest("UserName or Password is wrong.");
            }

            // Optionally, you can return a token here for JWT authentication.
            return Ok("Login successful");
        }
        [HttpPost("signout")]
        public async Task<IActionResult> SignOut()
        {
            await _accountRepository.SignOut();
            return Ok("Signout successful");
        }
        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            await _accountRepository.ForgetPassword(email);
            return Ok("Password reset email sent.");
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPassword fp)
        {
            var result = await _accountRepository.ResetPassword(fp);
            if (result.Succeeded)
            {
                return Ok("Password has been updated.");
            }
            else
            {
                return BadRequest("Error occurred.");
            }
        }


    }
}

using IdentityApi.Models.Interface;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;
using MailKit;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Net;
using System.Security.Claims;
namespace IdentityApi.Models.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        UserManager<CustomUser> _userManager;
        SignInManager<CustomUser> _signInManager;

        public AccountRepository(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> SignUp(RegisteredUser user)
        {
            var customUser = new CustomUser { UserName = user.UserName, Email = user.Email, FullName = user.Name };
            var result = await _userManager.CreateAsync(customUser, user.Password);
            if (result.Succeeded)
            {
                var role = "User";
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(customUser, role);
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(customUser);
                Console.WriteLine(token);
                var url = "https://localhost:7179";
                var verificationUrl = $"{url}/Account/VerifyEmail?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Test Project", "imhamzajamil@gmail.com"));
                message.To.Add(new MailboxAddress(user.Name, user.Email));
                message.Subject = "Verify Your Email Address";
                message.Body = new TextPart("plain")
                {
                    Text = verificationUrl
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.elasticemail.com", 2525, false);
                    client.Authenticate("imhamzajamil@gmail.com", "2A962F43019BB454DD8DBF75270A1D6A2122");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            return result;
        }
        public async Task<bool> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<SignInResult> Login(string userName, string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, Password, true, false);
            return result;
        }
        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var url = "https://localhost:7179";
                var verificationUrl = $"{url}/Account/GetEmail?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
                Console.WriteLine(token);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Test Project", "imhamzajamil@gmail.com"));
                message.To.Add(new MailboxAddress("User", email));
                message.Subject = "Verify Your Email Address";
                message.Body = new TextPart("plain")
                {
                    Text = verificationUrl
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.elasticemail.com", 2525, false);
                    client.Authenticate("imhamzajamil@gmail.com", "2A962F43019BB454DD8DBF75270A1D6A2122");
                    client.Send(message);
                    client.Disconnect(true);
                }

            }
        }
        public async Task<IdentityResult> ResetPassword(ForgetPassword fp)
        {
            var user = await _userManager.FindByEmailAsync(fp.Email);
            return await _userManager.ResetPasswordAsync(user, fp.Token, fp.Password);
        }
    }
}

using Microsoft.AspNetCore.Identity;

namespace IdentityApi.Models.Interface
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUp(RegisteredUser user);
        Task<bool> VerifyEmail(string email, string token);
        Task<SignInResult> Login(string userName, string Password);
        Task SignOut();
        Task ForgetPassword(string email);
        Task<IdentityResult> ResetPassword(ForgetPassword fp);
    }
}

using Microsoft.AspNetCore.Identity;

namespace IdentityApi.Models
{
    public class CustomUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}

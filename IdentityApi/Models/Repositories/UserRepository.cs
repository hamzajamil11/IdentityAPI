using IdentityApi.Models.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Models.Repositories
{
    public class UserRepository: IUserRepository
    {
        private UserManager<CustomUser> _userManager;

        public UserRepository( UserManager<CustomUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<CustomUser>> GetAllRegisteredUsers()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}

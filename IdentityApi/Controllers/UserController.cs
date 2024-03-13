using IdentityApi.Models.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using authorization = Microsoft.AspNetCore.Authorization;
namespace IdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepo; // Inject your user repository here

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        [authorization.Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllRegisteredUsers();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
    }
}

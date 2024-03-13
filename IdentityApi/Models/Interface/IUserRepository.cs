namespace IdentityApi.Models.Interface
{
    public interface IUserRepository
    {
        Task<List<CustomUser>> GetAllRegisteredUsers();
    }
}

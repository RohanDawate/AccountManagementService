using AccountManagementService.Business.Features.Users.Models;

namespace AccountManagementService.Business.Features.Users
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(CreateUserModel model);
    }
}

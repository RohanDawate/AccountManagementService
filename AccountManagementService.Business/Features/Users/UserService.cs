using AccountManagementService.Business.Features.Users.Models;

namespace AccountManagementService.Business.Features.Users
{
    public class UserService : IUserService
    {
        public Task<User> CreateAsync(CreateUserModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

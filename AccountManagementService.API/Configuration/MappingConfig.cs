using AccountManagementService.API.Features.Users;
using AccountManagementService.Business.Features.Users.Models;
using Mapster;

namespace AccountManagementService.API.Configuration
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Users
            TypeAdapterConfig<CreateUserRequest, CreateUserModel>
                .NewConfig()
                .Map(dest => dest.Name, src => src.Name.Trim())
                .Map(dest => dest.Email, src => src.Email.Trim())
                .Map(dest => dest.Age, src => src.Age);
        }
    }
}

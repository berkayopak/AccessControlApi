using AccessControlApi.Dto.Request;
using AccessControlApi.Dto.Response;

namespace AccessControlApi.Facade;

public interface IUserFacade
{
    public Task<AuthenticateResponse> Authenticate(AuthenticateRequest request);
    public Task<CreateUserResponse> CreateBasicUser(CreateUserRequest request, string roleName);
    public Task<AssignRoleToUserResponse> AssignRole(AssignRoleToUserRequest request);
}
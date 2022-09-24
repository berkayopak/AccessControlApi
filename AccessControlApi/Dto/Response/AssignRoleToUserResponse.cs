namespace AccessControlApi.Dto.Response;

public class AssignRoleToUserResponse
{
    public string UserId { get; }
    public string RoleName { get; }

    public AssignRoleToUserResponse(string userId, string roleName)
    {
        UserId = userId;
        RoleName = roleName;
    }
}